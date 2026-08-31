using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubastaYa.Core.Entities
{
    public class User
    {
        public int Id {  get; set; }

        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime Created { get; set; }

        // Navigation properties (Relationships)
        public Wallet Wallet { get; set; }
        public ICollection<Auction> publishedAuctions { get; set; } = new List<Auction>();
        public ICollection<Bid> pujasRealizadas { get; set; } = new List<Bid>();
    }
}
