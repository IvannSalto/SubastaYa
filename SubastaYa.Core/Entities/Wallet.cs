using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SubastaYa.Core.Entities
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }
        public User UserID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public double TotalBalance {  get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public double BalanceHeld { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public double AvailableBalance { get; set; }

        [Timestamp] // Optimistic Locking
        public int Version { get; set;}

        // Clave foránea y relación 1 a 1 con User
        public int UsuarioId { get; set; }
        public User Usuario { get; set; } = null!;

        // Propiedades de navegación
        public ICollection<TransactionLedger> Transacciones { get; set; } = new List<TransactionLedger>();
    }
}
