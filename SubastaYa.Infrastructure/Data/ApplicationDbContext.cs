using Microsoft.EntityFrameworkCore;
using SubastaYa.Core.Entities;
using System;

namespace SubastaYa.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Auction> Auctions { get; set; }
    public DbSet<Bid> Bids { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<TransactionLedger> TransactionLedgers { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    
}