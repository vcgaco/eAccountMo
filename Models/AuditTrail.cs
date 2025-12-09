using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eAccount.Models
{
    public class AuditTrail
    {
        [Key]
        public int Id { get; set; }

        public string TableName { get; set; }
        public string RecordId { get; set; }

        public string Action { get; set; } // INSERT, UPDATE, DELETE
        [Column(TypeName = "nvarchar(max)")]
        public string OldValues { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string NewValues { get; set; }

        public string ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.Now;
    }
}
