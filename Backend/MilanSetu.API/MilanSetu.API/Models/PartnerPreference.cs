using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    public class PartnerPreference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        // 1. Age Range
        public int MinAge { get; set; } = 21;
        public int MaxAge { get; set; } = 30;

        // 2. Height Range
        [MaxLength(20)]
        public string MinHeight { get; set; } = "5'0\"";
        [MaxLength(20)]
        public string MaxHeight { get; set; } = "5'10\"";

        // 3. Religion
        [MaxLength(100)]
        public string Religion { get; set; } = "Any Religion";

        // 4. Community / Caste
        [MaxLength(150)]
        public string Community { get; set; } = "Open to All / Caste No Bar";

        // 5. Mother Tongue
        [MaxLength(100)]
        public string MotherTongue { get; set; } = "Any Language";

        // 6. Education
        [MaxLength(150)]
        public string Education { get; set; } = "Graduate / Post Graduate & Above";

        // 7. Profession
        [MaxLength(150)]
        public string Profession { get; set; } = "Private / Govt / Business Professional";

        // 8. Income
        [MaxLength(50)]
        public string MinAnnualIncome { get; set; } = "₹5 Lakhs & Above";

        // 9. Location
        [MaxLength(200)]
        public string PreferredLocation { get; set; } = "Any Location in India / Open to Relocate";

        // 10. Marital Status
        [MaxLength(50)]
        public string MaritalStatus { get; set; } = "Never Married";

        // 11. Lifestyle
        [MaxLength(50)]
        public string Diet { get; set; } = "Vegetarian / Eggetarian";
        [MaxLength(50)]
        public string Drink { get; set; } = "Non-Drinker / Social Drinker";
        [MaxLength(50)]
        public string Smoke { get; set; } = "Non-Smoker";

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
