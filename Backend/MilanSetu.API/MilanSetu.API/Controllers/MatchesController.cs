using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilanSetu.API.DTOs;
using MilanSetu.API.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MilanSetu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchingService _matchingService;

        public MatchesController(IMatchingService matchingService)
        {
            _matchingService = matchingService;
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out var id))
            {
                return id;
            }
            return 1; // Fallback for guest/demo browsing
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardMatches()
        {
            var userId = GetCurrentUserId();
            var result = await _matchingService.GetDashboardMatchesAsync(userId);
            return Ok(result);
        }

        [HttpPost("shortlist/{targetUserId}")]
        public async Task<IActionResult> ToggleShortlist(int targetUserId)
        {
            var userId = GetCurrentUserId();
            var isAdded = await _matchingService.ToggleShortlistAsync(userId, targetUserId);
            return Ok(new
            {
                success = true,
                isShortlisted = isAdded,
                message = isAdded ? "Profile added to shortlist" : "Profile removed from shortlist"
            });
        }

        [HttpPost("view/{targetUserId}")]
        public async Task<IActionResult> RecordView(int targetUserId)
        {
            var userId = GetCurrentUserId();
            await _matchingService.RecordProfileViewAsync(userId, targetUserId);
            return Ok(new { success = true });
        }

        [HttpPost("interest")]
        public async Task<IActionResult> ExpressInterest([FromBody] SendInterestRequestDto req)
        {
            var userId = GetCurrentUserId();
            var interest = await _matchingService.ExpressInterestAsync(userId, req.TargetUserId, req.Message);
            return Ok(new
            {
                success = true,
                status = interest.Status,
                message = interest.Status == "Withdrawn" ? "Interest withdrawn" : "Interest sent successfully"
            });
        }

        [HttpPut("interest/{interestId}/respond")]
        public async Task<IActionResult> RespondToInterest(int interestId, [FromBody] RespondInterestDto req)
        {
            var userId = GetCurrentUserId();
            var updated = await _matchingService.RespondToInterestAsync(userId, interestId, req.Action);
            if (!updated)
            {
                return NotFound(new { message = "Interest record not found or unauthorized." });
            }

            return Ok(new
            {
                success = true,
                status = req.Action,
                message = $"Interest has been {req.Action.ToLower()}ed successfully."
            });
        }
    }
}
