using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SubastaYa.Core.Entities
{
    [Table("Wallets")]
    public class Wallet
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        public decimal TotalBalance => AvailableBalance + BalanceHeld;

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceHeld { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AvailableBalance { get; set; }

        [ConcurrencyCheck]
        public Guid Version { get; set; } = Guid.NewGuid();

        // Clave foránea y relación 1 a 1 con User
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        // Propiedades de navegación
        public ICollection<TransactionLedger> Transactions { get; set; }
    }
}
