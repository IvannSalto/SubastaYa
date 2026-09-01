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
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bid>()
            .HasOne(b => b.Auction)
            .WithMany(a => a.Bids)
            .HasForeignKey(b => b.AuctionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Bid>()
            .HasOne(b => b.Buyer)
            .WithMany(u => u.BidsPlaced) 
            .HasForeignKey(b => b.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}