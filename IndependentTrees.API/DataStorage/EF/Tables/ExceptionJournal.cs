using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IndependentTrees.API.DataStorage.EF.Tables
{
    [Index(nameof(EventID), IsUnique = true)]
    [Index(nameof(CreatedAt))]
    public class ExceptionJournal
    {
        public int Id { get; set; }

        public int EventID { get; set; }

        public DateTime CreatedAt { get; set; }

        [StringLength(256)]
        public string Path { get; set; }

        [StringLength(256)]
        public string Query { get; set; }

        [StringLength(256)]
        public string Body { get; set; }

        public string Exception { get; set; }
    }
}
