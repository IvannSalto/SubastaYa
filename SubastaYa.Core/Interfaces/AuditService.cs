namespace SubastaYa.Core.Interfaces
{
    public interface IAuditService
    {
        Task RegisterLogAsync(string entity, int entityId, string action, int? userId, object? detail = null);
    }
}