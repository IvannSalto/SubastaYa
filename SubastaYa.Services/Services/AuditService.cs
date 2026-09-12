using System.Text.Json;
using SubastaYa.Core.Entities;
using SubastaYa.Core.Interfaces;
using SubastaYa.Core.IRepositories;
using SubastaYa.Core.Utils;

namespace SubastaYa.Services.Services
{
    public class AuditService : IAuditService
    {
        private readonly IGenericRepository<AuditLog> _auditRepo;

        public AuditService(IGenericRepository<AuditLog> auditRepo)
        {
            _auditRepo = auditRepo;
        }

        public async Task RegisterLogAsync(string entity, int entityId, string action, int? userId, object? detail = null)
        {
            var log = new AuditLog
            {
                Entity = entity,
                EntityId = entityId,
                Action = action,
                UserId = userId,
                DetailJson = detail != null ? JsonSerializer.Serialize(detail) : string.Empty,
                Date = ArgTime.Now
            };

            await _auditRepo.AddAsync(log);
        }
    }
}