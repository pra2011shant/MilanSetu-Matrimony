using System;

namespace MilanSetu.API.DTOs
{
    public class PartnerPreferenceDto
    {
        public int UserId { get; set; }

        // 1. Age
        public int MinAge { get; set; } = 21;
        public int MaxAge { get; set; } = 30;

        // 2. Height
        public string MinHeight { get; set; } = "5'0\"";
        public string MaxHeight { get; set; } = "5'10\"";

        // 3. Religion
        public string Religion { get; set; } = "Any Religion";

        // 4. Community
        public string Community { get; set; } = "Open to All / Caste No Bar";

        // 5. Mother Tongue
        public string MotherTongue { get; set; } = "Any Language";

        // 6. Education
        public string Education { get; set; } = "Graduate / Post Graduate & Above";

        // 7. Profession
        public string Profession { get; set; } = "Private / Govt / Business Professional";

        // 8. Income
        public string MinAnnualIncome { get; set; } = "₹5 Lakhs & Above";

        // 9. Location
        public string PreferredLocation { get; set; } = "Any Location in India / Open to Relocate";

        // 10. Marital Status
        public string MaritalStatus { get; set; } = "Never Married";

        // 11. Lifestyle
        public string Diet { get; set; } = "Vegetarian / Eggetarian";
        public string Drink { get; set; } = "Non-Drinker / Social Drinker";
        public string Smoke { get; set; } = "Non-Smoker";

        public DateTime? UpdatedAt { get; set; }
    }
}
