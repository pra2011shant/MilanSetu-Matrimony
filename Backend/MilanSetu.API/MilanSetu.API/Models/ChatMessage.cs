using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    /// <summary>
    /// Database Table Mapping: [dbo].[ChatMessages]
    /// Direct 1-to-1 conversation messages delivered via SignalR real-time hubs.
    /// </summary>
    [Table("ChatMessages")]
    public class ChatMessage
    {
        /// <summary>
        /// Column: Id (INT, Primary Key, Identity)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Column: SenderId (INT, Foreign Key -> [dbo].[Users].[Id])
        /// </summary>
        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        public User Sender { get; set; } = null!;

        /// <summary>
        /// Column: ReceiverId (INT, Foreign Key -> [dbo].[Users].[Id])
        /// </summary>
        public int ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; } = null!;

        /// <summary>
        /// Column: Content (NVARCHAR(2000), NOT NULL) - Message text body
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Column: IsRead (BIT, NOT NULL, Default: 0) - Read receipt flag
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Column: SentAt (DATETIME2, NOT NULL)
        /// </summary>
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Column: ReadAt (DATETIME2, NULL) - Timestamp when recipient opened the conversation
        /// </summary>
        public DateTime? ReadAt { get; set; }
    }
}
