using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SubastaYa.Core.Entities
{
    [Table("Bids")]
    public class Bid
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int AuctionId { get; set; }
        
        [Required]
        public int BuyerId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public DateTime BidDate { get; set; }
        
        [ForeignKey("AuctionId")]
        public Auction Auction { get; set; }

        [ForeignKey("BuyerId")]
        public User Buyer { get; set; }
    }
}
