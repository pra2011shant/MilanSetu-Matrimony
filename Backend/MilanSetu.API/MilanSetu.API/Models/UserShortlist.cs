using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    /// <summary>
    /// Database Table Mapping: [dbo].[UserShortlists]
    /// Stores shortlisted/bookmarked profiles by users for quick tracking and future outreach.
    /// </summary>
    [Table("UserShortlists")]
    public class UserShortlist
    {
        /// <summary>
        /// Column: Id (INT, Primary Key, Identity)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Column: UserId (INT, Foreign Key -> [dbo].[Users].[Id])
        /// </summary>
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        /// <summary>
        /// Column: ShortlistedUserId (INT, Foreign Key -> [dbo].[Users].[Id])
        /// </summary>
        public int ShortlistedUserId { get; set; }
        [ForeignKey("ShortlistedUserId")]
        public User ShortlistedUser { get; set; } = null!;

        /// <summary>
        /// Column: ShortlistedAt (DATETIME2, NOT NULL)
        /// </summary>
        public DateTime ShortlistedAt { get; set; } = DateTime.UtcNow;
    }
}
