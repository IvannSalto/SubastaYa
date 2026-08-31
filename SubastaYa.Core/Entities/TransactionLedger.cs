using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubastaYa.Core.Entities
{
    [Table("TransactionLedgers")]
    public class TransactionLedger
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WalletId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public int? AuctionId { get; set; } 

        [ForeignKey("WalletId")]
        public Wallet Wallet { get; set; }
    }
}