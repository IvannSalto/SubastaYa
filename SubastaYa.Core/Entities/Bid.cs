using System;
using System.Collections.Generic;
using System.Text;

namespace SubastaYa.Core.Entities
{
    public class Bid
    {
        public int Id { get; set; }
        public int SubastaId { get; set; }
        public int BuyerId { get; set; }
        public decimal amount { get; set; }
        public DateTime BidDate { get; set; }
    }
}
