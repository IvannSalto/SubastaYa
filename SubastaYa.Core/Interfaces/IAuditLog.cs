using SubastaYa.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubastaYa.Core.Interfaces
{
    public interface IAuditLog
    {
        // registrar un evento en el log de auditoría
        Task LogAsync(string entity, string entityId, string action, int? userId, string? details);

        // Consultar eventos auditados para reportes o panel de control
        Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entity, string entityId);
    }
}
