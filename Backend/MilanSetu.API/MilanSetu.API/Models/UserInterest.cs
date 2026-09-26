using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    /// <summary>
    /// Database Table Mapping: [dbo].[UserInterests]
    /// Manages Express Interest proposals, connect requests, and response status (Pending, Accepted, Declined).
    /// </summary>
    [Table("UserInterests")]
    public class UserInterest
    {
        /// <summary>
        /// Column: Id (INT, Primary Key, Identity)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Column: SenderUserId (INT, Foreign Key -> [dbo].[Users].[Id])
        /// </summary>
        public int SenderUserId { get; set; }
        [ForeignKey("SenderUserId")]
        public User SenderUser { get; set; } = null!;

        /// <summary>
        /// Column: ReceiverUserId (INT, Foreign Key -> [dbo].[Users].[Id])
        /// </summary>
        public int ReceiverUserId { get; set; }
        [ForeignKey("ReceiverUserId")]
        public User ReceiverUser { get; set; } = null!;

        /// <summary>
        /// Column: Status (NVARCHAR(30), NOT NULL) - 'Pending', 'Accepted', 'Declined', 'Withdrawn'
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Column: CustomMessage (NVARCHAR(MAX), NULL) - Optional personalized introductory note
        /// </summary>
        public string? CustomMessage { get; set; }

        /// <summary>
        /// Column: SentAt (DATETIME2, NOT NULL)
        /// </summary>
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Column: RespondedAt (DATETIME2, NULL)
        /// </summary>
        public DateTime? RespondedAt { get; set; }
    }
}
