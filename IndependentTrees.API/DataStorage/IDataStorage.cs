namespace IndependentTrees.API.DataStorage
{
    public interface IDataStorage
    {
        int GetNextEventID();

        Task WriteExceptionAsync(
            Exception exception,
            int eventID,
            DateTime createdAt,
            string path,
            string body,
            string query);

        Task<Models.Journal?> GetJournalAsync(int id);

        Task<Models.Range<Models.JournalInfo>> GetJournalsAsync(
            int skip,
            int take,
            Models.JournalFilter filter);

        Task<Models.Node> GetOrCreateTreeAsync(string treeName);

        Task CreateNodeAsync(string treeName, int parentNodeId, string nodeName);

        Task DeleteNodeAsync(string treeName, int nodeId);

        Task RenameNodeAsync(string treeName, int nodeId, string newNodeName);
    }
}
