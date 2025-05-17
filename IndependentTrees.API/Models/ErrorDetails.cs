using System.Text.Json;

namespace IndependentTrees.API.Models
{
    public class ErrorDetails
    {
        public ErrorDetails(string type, string id, ErrorData data)
        {
            Type = type;
            Id = id;
            Data = data;
        }

        public string Type { get; }
        
        public string Id { get; }

        public ErrorData Data { get; }
    }

    public class ErrorData
    {
        public ErrorData(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
