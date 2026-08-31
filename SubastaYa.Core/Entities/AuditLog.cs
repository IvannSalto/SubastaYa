using System;
using System.Collections.Generic;
using System.Text;

namespace SubastaYa.Core.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Entity {  get; set; }
        public int EntityId { get; set; }
        public string Action { get; set; }
        public int UserId { get; set; }
        public string DetailJson {  get; set; }
        public DateTime Date {  get; set; }
    }
}
