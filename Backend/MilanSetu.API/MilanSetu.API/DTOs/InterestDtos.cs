using System;
using System.Collections.Generic;

namespace MilanSetu.API.DTOs
{
    public class InterestItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ProfileId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Height { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public string Caste { get; set; } = string.Empty;
        public string MotherTongue { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string AnnualIncome { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool Verified { get; set; } = true;
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Declined, Withdrawn
        public string? CustomMessage { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public int MatchScore { get; set; } = 88;

        // Unlocked Contact Information (Populated ONLY when Status == 'Accepted')
        public bool IsCommunicationUnlocked { get; set; }
        public string? ContactMobile { get; set; }
        public string? ContactEmail { get; set; }
        public string? PreferredCallTime { get; set; } = "7:00 PM - 9:00 PM";
    }

    public class InterestCountsDto
    {
        public int PendingReceived { get; set; }
        public int AcceptedConnected { get; set; }
        public int PendingSent { get; set; }
        public int TotalReceived { get; set; }
    }

    public class SendInterestDto
    {
        public int ReceiverUserId { get; set; }
        public string? CustomMessage { get; set; }
    }

    public class RespondInterestRequest
    {
        public string Action { get; set; } = "Accepted"; // Accepted, Declined
    }
}
