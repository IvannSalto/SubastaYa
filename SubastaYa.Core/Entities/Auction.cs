using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SubastaYa.Core.Entities
{
    [Table("Auctions")]
    public class Auction
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int Seller {  get; set; }
        
        [Required]
        public int CategoryId {  get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Url]
        public string UrlImage { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumIncrement {  get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [Required]
        [MaxLength(20)]
        public String State { get; set; }
        
        [ConcurrencyCheck]
        public int Version { get; set; }
        
        public Category Category { get; set; }
        
        public ICollection<Bid> Bids { get; set; }
    }
}
