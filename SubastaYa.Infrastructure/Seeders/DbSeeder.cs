using Microsoft.EntityFrameworkCore;
using SubastaYa.Core.Entities;
using SubastaYa.Infrastructure.Data;
using BCrypt.Net;

namespace SubastaYa.Infrastructure.Seeders
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureDeletedAsync(); //comentarla en produccion
            await context.Database.MigrateAsync();
            
            context.Bids.RemoveRange(context.Bids);
            context.Auctions.RemoveRange(context.Auctions);
            context.Wallets.RemoveRange(context.Wallets);
            context.Users.RemoveRange(context.Users);
            context.Categories.RemoveRange(context.Categories);
            await context.SaveChangesAsync();
            
            if (await context.Users.AnyAsync()) return;

            // ------- Categorias ---------
            var catTecno = new Category { Name = "Tecnología", UrlIcon = "https://via.placeholder.com/150" };
            var catColec = new Category { Name = "Coleccionables", UrlIcon = "https://via.placeholder.com/150" };
            var catIndum = new Category { Name = "Indumentaria", UrlIcon = "https://via.placeholder.com/150" };
            var catVehic = new Category { Name = "Vehículos", UrlIcon = "https://via.placeholder.com/150" };
            
            await context.Categories.AddRangeAsync(catTecno, catColec, catIndum, catVehic);
            await context.SaveChangesAsync();

            // ------ Usuarios -------
            string hash = BCrypt.Net.BCrypt.HashPassword("123");

            var vendedor = new User { Name = "Vendedor Test", Email = "vendedor@test.com", PasswordHash = hash, Created = DateTime.UtcNow };
            var comprador1 = new User { Name = "Comprador Lider", Email = "comprador1@test.com", PasswordHash = hash, Created = DateTime.UtcNow };
            var comprador2 = new User { Name = "Comprador Habilitado", Email = "comprador2@test.com", PasswordHash = hash, Created = DateTime.UtcNow };
            var sinFondos = new User { Name = "Usuario Pobre", Email = "sinfondos@test.com", PasswordHash = hash, Created = DateTime.UtcNow };
            var compradorDummy = new User { Name = "Comprador Extra", Email = "dummy@test.com", PasswordHash = hash, Created = DateTime.UtcNow }; // Para ganar la subasta vencida

            await context.Users.AddRangeAsync(vendedor, comprador1, comprador2, sinFondos, compradorDummy);
            await context.SaveChangesAsync();

            // ----- BILLETERAS ---
            var wallets = new List<Wallet>
            {
                new Wallet { UserId = vendedor.Id, AvailableBalance = 0, BalanceHeld = 0 },
                new Wallet { UserId = comprador1.Id, AvailableBalance = 105000, BalanceHeld = 45000 }, // Total: 150.000
                new Wallet { UserId = comprador2.Id, AvailableBalance = 200000, BalanceHeld = 0 },     // Total: 200.000
                new Wallet { UserId = sinFondos.Id, AvailableBalance = 500, BalanceHeld = 0 },         // Total: 500 
                new Wallet { UserId = compradorDummy.Id, AvailableBalance = 0, BalanceHeld = 100000 }
            };
            
            await context.Wallets.AddRangeAsync(wallets);
            await context.SaveChangesAsync();

            // ----- Subastas -------------
            var subastas = new List<Auction>
            {
                // Cierra en 25 min (Líder actual $45.000)
                new Auction { Title = "Laptop Gamer", Description = "Descripcion generica.", CategoryId = catTecno.Id, UrlImage = "https://picsum.photos/800/600", SellerId = vendedor.Id, BasePrice = 30000, MinimumIncrement = 1000, State = "Active", StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddMinutes(25) },
                
                // Cierra en 90 segundos (prueba antisnipig)
                new Auction { Title = "Reloj Antiguo", Description = "Descripcion generica.", CategoryId = catColec.Id, UrlImage = "https://picsum.photos/800/600", SellerId = vendedor.Id, BasePrice = 5000, MinimumIncrement = 500, State = "Active", StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddSeconds(90) },
                
                // Inicio programado a +24 hs (Pujas bloqueadas)
                new Auction { Title = "Zapatillas Limitadas", Description = "Descripcion generica.", CategoryId = catIndum.Id, UrlImage = "https://picsum.photos/800/600", SellerId = vendedor.Id, BasePrice = 20000, MinimumIncrement = 1000, State = "Pending", StartDate = DateTime.UtcNow.AddHours(24), EndDate = DateTime.UtcNow.AddDays(3) },
                
                // Vencida con ganador: Fecha fin pasada + puja ganadora
                new Auction { Title = "Auto Clásico", Description = "Descripcion generica.", CategoryId = catVehic.Id, UrlImage = "https://picsum.photos/800/600", SellerId = vendedor.Id, BasePrice = 80000, MinimumIncrement = 5000, State = "Active", StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddHours(-1) },
                
                // Vencida desierta: fecha fin pasada sin pujas
                new Auction { Title = "Camiseta Firmada", Description = "Descripcion generica.", CategoryId = catColec.Id, UrlImage = "https://picsum.photos/800/600", SellerId = vendedor.Id, BasePrice = 150000, MinimumIncrement = 10000, State = "Active", StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddHours(-2) }
            };

            await context.Auctions.AddRangeAsync(subastas);
            await context.SaveChangesAsync();

            // --- PUJAS Y REGISTROS ------
            var bids = new List<Bid>
            {
                // Historial de 2 ofertas en la Subasta "Activa Estándar"
                new Bid { AuctionId = subastas[0].Id, BuyerId = comprador2.Id, Amount = 35000, BidDate = DateTime.UtcNow.AddMinutes(-30) }, // Puja antigua
                new Bid { AuctionId = subastas[0].Id, BuyerId = comprador1.Id, Amount = 45000, BidDate = DateTime.UtcNow.AddMinutes(-5) },  // Puja líder que retiene los 45k

                // Oferta ganadora en la Subasta "Vencida con ganador"
                new Bid { AuctionId = subastas[3].Id, BuyerId = compradorDummy.Id, Amount = 100000, BidDate = DateTime.UtcNow.AddHours(-2) }
            };

            await context.Bids.AddRangeAsync(bids);
            await context.SaveChangesAsync();
        }
    }
}