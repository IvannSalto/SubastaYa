using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubastaYa.Core.Entities
{
    public class User
    {
        public int Id {  get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime Created { get; set; }

        // Navigation properties (Relationships)
        public Wallet Wallet { get; set; }
        public ICollection<Auction> publishedAuctions { get; set; }
        public ICollection<Bid> bidsPlaced { get; set; } 
    }
}
