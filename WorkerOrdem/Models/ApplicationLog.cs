using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkerOrdem.Models
{
    [Table("ApplicationLogs")]
    public class ApplicationLog
    {
        [Key]
        public long Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string CorrelationId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ServiceName { get; set; }
        
        [Required]
        [StringLength(20)]
        public string LogLevel { get; set; }
        
        [Required]
        public string Message { get; set; }
        
        public string? Exception { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        [StringLength(100)]
        public string? UserId { get; set; }
        
        [StringLength(500)]
        public string? RequestPath { get; set; }
        
        [StringLength(10)]
        public string? HttpMethod { get; set; }
        
        public int? Duration { get; set; }
    }
}
