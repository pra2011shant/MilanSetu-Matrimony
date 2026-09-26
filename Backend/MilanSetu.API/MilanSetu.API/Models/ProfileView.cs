using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    public class ProfileView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ViewerUserId { get; set; }
        [ForeignKey("ViewerUserId")]
        public User ViewerUser { get; set; } = null!;

        public int ViewedUserId { get; set; }
        [ForeignKey("ViewedUserId")]
        public User ViewedUser { get; set; } = null!;

        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }
}
