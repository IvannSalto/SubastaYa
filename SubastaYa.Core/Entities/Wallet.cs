using System;
using System.Collections.Generic;
using System.Text;

namespace SubastaYa.Core.Entities
{
    internal class Wallet
    {
        public int Id { get; set; }
        public User UserID { get; set; }
        public double TotalBalance {  get; set; }
        public double BalanceHeld { get; set; }
        public double AvailableBalance { get; set; }
        public int Version { get; set;}
    }
}
