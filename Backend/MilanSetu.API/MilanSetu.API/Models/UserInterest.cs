using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    public class UserInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SenderUserId { get; set; }
        [ForeignKey("SenderUserId")]
        public User SenderUser { get; set; } = null!;

        public int ReceiverUserId { get; set; }
        [ForeignKey("ReceiverUserId")]
        public User ReceiverUser { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Declined, Withdrawn

        public string? CustomMessage { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public DateTime? RespondedAt { get; set; }
    }
}
