using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    public class ChatMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        public User Sender { get; set; } = null!;

        public int ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; } = null!;

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }
    }
}
