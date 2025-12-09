using eAccount.Data;
using eAccount.Models;
using Newtonsoft.Json;
using System;
namespace eAccount.Services
{
    public interface IAuditService
    {
        Task LogAsync(string table, int recordId, string action,
                      object? oldData, object? newData, string user);
    }

    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        public AuditService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string table, int recordId, string action,
                                   object? oldData, object? newData, string user)
        {
            var audit = new AuditTrail
            {
                TableName = table,
                RecordId = recordId.ToString(),
                Action = action,
                OldValues = oldData != null ? JsonConvert.SerializeObject(oldData) : "",
                NewValues = newData != null ? JsonConvert.SerializeObject(newData) : "",
                ChangedBy = string.IsNullOrEmpty(user) ? "System" : user,
                ChangedAt = DateTime.Now
            };

            _context.AuditTrails.Add(audit);
            await _context.SaveChangesAsync();
        }
    }
}
