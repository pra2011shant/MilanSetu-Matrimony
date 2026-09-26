using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    /// <summary>
    /// Database Table Mapping: [dbo].[Notifications]
    /// Real-time alert records dispatched to users upon incoming interests, profile views, chat messages, or verification changes.
    /// </summary>
    [Table("Notifications")]
    public class Notification
    {
        /// <summary>
        /// Column: Id (INT, Primary Key, Identity)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Column: UserId (INT, Foreign Key -> [dbo].[Users].[Id]) - Recipient of the notification
        /// </summary>
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        /// <summary>
        /// Column: Type (NVARCHAR(50), NOT NULL) - 'Interest', 'ProfileView', 'Message', 'Verification', 'System'
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "Interest";

        /// <summary>
        /// Column: Title (NVARCHAR(200), NOT NULL) - Short heading of the alert
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Column: Message (NVARCHAR(500), NOT NULL) - Detailed message body
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Column: ActionUrl (NVARCHAR(250), NULL) - Frontend route link (e.g., '/interests', '/chat')
        /// </summary>
        [MaxLength(250)]
        public string? ActionUrl { get; set; }

        /// <summary>
        /// Column: AvatarUrl (NVARCHAR(MAX), NULL) - Photo of actor who triggered the alert
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Column: IsRead (BIT, NOT NULL, Default: 0)
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Column: CreatedAt (DATETIME2, NOT NULL)
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
