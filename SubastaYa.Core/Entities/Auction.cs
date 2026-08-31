using System;
using System.Collections.Generic;
using System.Text;

namespace SubastaYa.Core.Entities
{
    public class Auction
    {
        public int Id { get; set; }
        public int Seller {  get; set; }
        public int Category {  get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url_image { get; set; }
        public double BasePrice { get; set; }
        public double MinimumIncrement {  get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String state { get; set; }
        public int Version { get; set; }
    }
}
