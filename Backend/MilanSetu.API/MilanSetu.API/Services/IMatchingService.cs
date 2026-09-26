using MilanSetu.API.DTOs;
using MilanSetu.API.Models;
using System.Threading.Tasks;

namespace MilanSetu.API.Services
{
    public interface IMatchingService
    {
        Task<DashboardMatchesDto> GetDashboardMatchesAsync(int currentUserId);
        MatchedProfileDto CalculateMatch(PartnerPreference? pref, User user, UserProfile? profile);
        Task<bool> ToggleShortlistAsync(int currentUserId, int targetUserId);
        Task<bool> RecordProfileViewAsync(int currentUserId, int targetUserId);
        Task<UserInterest> ExpressInterestAsync(int currentUserId, int targetUserId, string? message);
        Task<bool> RespondToInterestAsync(int currentUserId, int interestId, string action);
    }
}
