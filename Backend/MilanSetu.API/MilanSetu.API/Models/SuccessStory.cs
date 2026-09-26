using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    /// <summary>
    /// Database Table Mapping: [dbo].[SuccessStories]
    /// Stores real/verified wedding success stories of couples who met via MilanSetu.
    /// </summary>
    [Table("SuccessStories")]
    public class SuccessStory
    {
        /// <summary>
        /// Column: Id (INT, Primary Key, Identity)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Column: CoupleName (NVARCHAR(150), NOT NULL) - e.g. "Vikram & Radhika"
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string CoupleName { get; set; } = string.Empty;

        /// <summary>
        /// Column: WeddingDate (NVARCHAR(100), NOT NULL) - e.g. "December 2025"
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string WeddingDate { get; set; } = string.Empty;

        /// <summary>
        /// Column: Location (NVARCHAR(150), NOT NULL) - Wedding city/venue
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Column: ImageUrl (NVARCHAR(MAX), NOT NULL) - Photo URL of the couple
        /// </summary>
        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Column: Quote (NVARCHAR(500), NOT NULL) - Couple testimonial quote
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Quote { get; set; } = string.Empty;

        /// <summary>
        /// Column: StorySnippet (NVARCHAR(1000), NOT NULL) - Detailed story summary
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string StorySnippet { get; set; } = string.Empty;

        /// <summary>
        /// Column: IsActive (BIT, NOT NULL, Default: 1)
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Column: SortOrder (INT, NOT NULL, Default: 0)
        /// </summary>
        public int SortOrder { get; set; } = 0;

        /// <summary>
        /// Column: CreatedAt (DATETIME2, NOT NULL)
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
