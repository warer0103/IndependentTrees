using IndependentTrees.API.DataStorage.EF.Tables;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace IndependentTrees.API.DataStorage.EF
{
    public static class Convertor
    {
        public static Models.JournalInfo ToModelJournalInfo(ExceptionJournal journal) =>
            new Models.JournalInfo
            {
                Id = journal.Id,
                EventID = journal.EventID,
                CreatedAt = journal.CreatedAt,
            };

        public static Models.Journal ToModelJournal(ExceptionJournal journal) =>
            new Models.Journal
            {
                Id = journal.Id,
                EventID = journal.EventID,
                CreatedAt = journal.CreatedAt,
                Text = ToJournalText(journal)
            };

        public static string ToJournalText(ExceptionJournal journal)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Created at {journal.CreatedAt:yyyy:MM:dd HH:mm:ss.fff}");
            sb.AppendLine($"Request ID = {journal.EventID}");
            if (!string.IsNullOrWhiteSpace(journal.Path))
                sb.AppendLine($"Path = {journal.Path}");

            if (!string.IsNullOrWhiteSpace(journal.Query))
            {
                var queryDictionary = QueryHelpers.ParseQuery(journal.Query);
                foreach (var param in queryDictionary)
                    sb.AppendLine($"{param.Key} = {param.Value}");
            }

            if (!string.IsNullOrWhiteSpace(journal.Body))
                sb.AppendLine(journal.Body);

            sb.AppendLine(journal.Exception);

            return sb.ToString();
        }

        public static Models.Node ToModelNode(TreeNode treeNode) =>
            new Models.Node
            {
                Id = treeNode.Id,
                Name = treeNode.Name,
                Children = treeNode.Children.Select(n => ToModelNode(n)).ToArray()
            };
    }
}
