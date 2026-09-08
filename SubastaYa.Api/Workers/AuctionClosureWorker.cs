using Microsoft.EntityFrameworkCore;
using SubastaYa.Core.Interfaces;
using SubastaYa.Core.Utils;
using SubastaYa.Infrastructure.Data;

namespace SubastaYa.Api.Workers
{
    public class AuctionClosureWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AuctionClosureWorker> _logger;

        public AuctionClosureWorker(IServiceProvider serviceProvider, ILogger<AuctionClosureWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker de Subastas iniciado. Esperando vencimientos...");


            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(20)); //revisa cada 20 segundos

            // Este ciclo se repite infinitamente mientras la API este encendida
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await ProcessExpiredAuctionsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error critico en el Worker al procesar subastas vencidas.");
                }
            }
        }

        private async Task ProcessExpiredAuctionsAsync()
        {
            using var scope = _serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var auctionService = scope.ServiceProvider.GetRequiredService<IAuctionService>();

            // Buscamos todas las subastas Activas cuya fecha de fin ya paso
            var expiredAuctionsIds = await dbContext.Auctions
                .Where(a => a.State == "Active" && a.EndDate <= ArgTime.Now)
                .Select(a => a.Id)
                .ToListAsync();

            if (expiredAuctionsIds.Any())
            {
                _logger.LogInformation($"Se encontraron {expiredAuctionsIds.Count} subasta(s) vencida(s). Procesando cierre...");

                foreach (var auctionId in expiredAuctionsIds)
                {
                    await auctionService.ProcessAuctionClosureAsync(auctionId, null);
                    _logger.LogInformation($"Subasta {auctionId} cerrada y liquidada automaticamente.");
                }
            }
        }
    }
}