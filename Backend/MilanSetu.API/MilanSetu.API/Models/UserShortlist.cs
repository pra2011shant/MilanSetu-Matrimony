using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    public class UserShortlist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public int ShortlistedUserId { get; set; }
        [ForeignKey("ShortlistedUserId")]
        public User ShortlistedUser { get; set; } = null!;

        public DateTime ShortlistedAt { get; set; } = DateTime.UtcNow;
    }
}
