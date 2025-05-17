using Microsoft.EntityFrameworkCore;
using IndependentTrees.API.Models;
using IndependentTrees.API.DataStorage.EF.Tables;
using IndependentTrees.API.Exceptions;

namespace IndependentTrees.API.DataStorage.EF
{
    public class EFDataStorage : IDataStorage
    {
        private readonly DbContextOptions _dbContextOptions;
        private readonly IdHolder _evetnIdHolder = new IdHolder();

        public EFDataStorage(DbContextOptions dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public void Init()
        {
            using (var db = new IndependentTreesContext(_dbContextOptions))
            {
                var isCreated = db.Database.EnsureCreated();
                if (!isCreated)
                {
                    var maxEventID = db.ExceptionJournals.Max(j => (int?)j.EventID) ?? 0;
                    _evetnIdHolder.SetID(maxEventID);
                }
            }
        }

        public int GetNextEventID() => _evetnIdHolder.GetNextID();

        public async Task WriteExceptionAsync(
            Exception exception,
            int eventID,
            DateTime createdAt,
            string path,
            string body,
            string query)
        {
            using (var db = new IndependentTreesContext(_dbContextOptions))
            {
                await db.ExceptionJournals
                    .AddAsync(new Tables.ExceptionJournal
                    {
                        EventID = eventID,
                        CreatedAt = createdAt,
                        Exception = exception.ToString(),
                        Path = path,
                        Body = body,
                        Query = query,
                    });

                await db.SaveChangesAsync();
            }
        }

        public async Task<Models.Journal?> GetJournalAsync(int id)
        {
            using (var db = new IndependentTreesContext(_dbContextOptions))
            {
                var journal = await db.ExceptionJournals.AsNoTracking()
                    .SingleOrDefaultAsync(j => j.Id == id);
                return journal == null ? null : Convertor.ToModelJournal(journal);
            }
        }

        public async Task<Models.Range<Models.JournalInfo>> GetJournalsAsync(
            int skip,
            int take,
            Models.JournalFilter filter)
        {
            using (var db = new IndependentTreesContext(_dbContextOptions))
            {
                var query = db.ExceptionJournals.AsQueryable();
                var from = filter.From ?? DateTime.MinValue;
                var to = filter.To ?? DateTime.MaxValue;

                query = query.Where(j => j.CreatedAt >= from && j.CreatedAt <= to);
                if (!string.IsNullOrWhiteSpace(filter.Search))
                    query = query
                        .Where(j => j.Path.Contains(filter.Search)
                            || j.Query.Contains(filter.Search)
                            || j.Body.Contains(filter.Search)
                            || j.Exception.Contains(filter.Search));

                var count = await query.CountAsync();
                var items = await query
                    .OrderBy(j => j.Id)
                    .Skip(skip)
                    .Take(take)
                    .ToArrayAsync();

                return new Range<JournalInfo>
                {
                    Skip = skip,
                    Count = count,
                    Items = items.Select(j => Convertor.ToModelJournalInfo(j)).ToArray()
                };

            }
        }

        public async Task<Models.Node> GetOrCreateTreeAsync(string treeName)
        {
            using (var db = new IndependentTreesContext(_dbContextOptions))
            {
                var node = await GetTree(treeName, db);
                if (node != null)
                    return node;

                return await CreateTree(treeName, db);
            }
        }

        private async Task<Models.Node?> GetTree(string treeName, IndependentTreesContext db)
        {
            var node = await GetTreeNode(treeName, db);

            if (node == null)
                return null;

            var allChilds = await db.TreeNodes.AsNoTracking()
                .Where(n => n.TreeId == node.Id)
                .ToArrayAsync();

            BuildTree(node, allChilds);

            return Convertor.ToModelNode(node);
        }

        private void BuildTree(TreeNode parent, TreeNode[] allChilds)
        {
            parent.Children = allChilds.Where(n => n.ParentId == parent.Id).ToArray();

            foreach (var child in parent.Children)
                BuildTree(child, allChilds);
        }

        private async Task<Models.Node> CreateTree(string treeName, IndependentTreesContext db)
        {
            var newNode = new TreeNode { Name = treeName, Children = Array.Empty<TreeNode>() };
            await db.TreeNodes.AddAsync(newNode);
            await db.SaveChangesAsync();
            return Convertor.ToModelNode(newNode);
        }

        public async Task CreateNodeAsync(string treeName, int parentNodeId, string nodeName)
        {
            using (var db = new IndependentTreesContext(_dbContextOptions))
            {
                var treeNode = await GetEnsureTreeNode(treeName, db);
                await EnsureNoDuplicateNames(treeNode.Id, nodeName, db);

                var parentNode = await GetEnsureNode(parentNodeId, treeNode.Id, db);

                await db.TreeNodes.AddAsync(new TreeNode { Name = nodeName, ParentId = parentNode.Id, TreeId = treeNode.Id });
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteNodeAsync(string treeName, int nodeId)
        {
            using (var db = new IndependentTreesContext(_dbContextOptions))
            {
                var treeNode = await GetEnsureTreeNode(treeName, db);

                var nodeToDelete = await GetEnsureNode(nodeId, treeNode.Id, db);

                await EnsureNoChildren(treeNode.Id, nodeId, db);

                db.TreeNodes.Remove(nodeToDelete);
                await db.SaveChangesAsync();
            }
        }
 
        public async Task RenameNodeAsync(
            string treeName,
            int nodeId,
            string newNodeName)
        {
            using (var db = new IndependentTreesContext(_dbContextOptions))
            {
                var treeNode = await GetEnsureTreeNode(treeName, db);
                await EnsureNoDuplicateNames(treeNode.Id, newNodeName, db);

                var node = await GetEnsureNode(nodeId, treeNode.Id, db);
              
                node.Name = newNodeName;
                db.Update(node);
                await db.SaveChangesAsync();
            }
        }

        private async Task<TreeNode?> GetTreeNode(string treeName, IndependentTreesContext db) =>
            await db.TreeNodes.AsNoTracking().SingleOrDefaultAsync(n => n.ParentId == null && n.Name == treeName);

        private async Task<TreeNode> GetEnsureTreeNode(string treeName, IndependentTreesContext db)
        {
            var treeNode = await GetTreeNode(treeName, db);
            return treeNode ?? throw new SecureException($"{treeName} not found");
        }

        private async Task EnsureNoDuplicateNames(int treeId, string name, IndependentTreesContext db)
        {
            if (await db.TreeNodes.AsNoTracking()
                 .AnyAsync(n => (n.TreeId == treeId || n.Id == treeId) && n.Name == name))
                throw new SecureException($"Duplicated name");
        }

        private async Task EnsureNoChildren(int treeId, int parentID, IndependentTreesContext db)
        {
            if (await db.TreeNodes.AsNoTracking()
                   .AnyAsync(n => n.TreeId == treeId && n.ParentId == parentID))
                throw new SecureException("The node has children");
        }

        private async Task<TreeNode> GetEnsureNode(int nodeID, int treeId, IndependentTreesContext db)
        {
            var node = await db.TreeNodes.AsNoTracking()
                    .SingleOrDefaultAsync(n => n.Id == nodeID && (n.TreeId == treeId || n.Id == treeId));

            return node ?? throw new SecureException($"Node with ID = {nodeID} was not found");
        }
    }
}
