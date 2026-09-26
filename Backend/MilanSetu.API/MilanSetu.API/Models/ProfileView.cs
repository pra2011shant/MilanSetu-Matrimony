using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    /// <summary>
    /// Database Table Mapping: [dbo].[ProfileViews]
    /// Tracks profile visitors to display "Who Viewed My Profile" insights on user dashboards.
    /// </summary>
    [Table("ProfileViews")]
    public class ProfileView
    {
        /// <summary>
        /// Column: Id (INT, Primary Key, Identity)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Column: ViewerUserId (INT, Foreign Key -> [dbo].[Users].[Id])
        /// </summary>
        public int ViewerUserId { get; set; }
        [ForeignKey("ViewerUserId")]
        public User ViewerUser { get; set; } = null!;

        /// <summary>
        /// Column: ViewedUserId (INT, Foreign Key -> [dbo].[Users].[Id])
        /// </summary>
        public int ViewedUserId { get; set; }
        [ForeignKey("ViewedUserId")]
        public User ViewedUser { get; set; } = null!;

        /// <summary>
        /// Column: ViewedAt (DATETIME2, NOT NULL)
        /// </summary>
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }
}
