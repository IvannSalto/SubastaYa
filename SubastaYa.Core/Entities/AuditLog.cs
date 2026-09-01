using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text;

namespace SubastaYa.Core.Entities
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Entity {  get; set; }
        
        [Required]
        public int EntityId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Action { get; set; }
        
        public int? UserId { get; set; }
        public string DetailJson {  get; set; }
        public DateTime Date {  get; set; }
    }
}
