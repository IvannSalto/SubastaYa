using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SubastaYa.Core.Entities
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        public int Id { get; set;}
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set;}
        
        [MaxLength(255)]
        public string UrlIcon { get; set;}
        
        public ICollection<Auction> Auctions { get; set; }
    }
}
