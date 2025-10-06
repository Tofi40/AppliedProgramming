using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomieMatch.Application.Matching;
using RoomieMatch.Domain.Entities;

namespace RoomieMatch.Api.Controllers;

[ApiController]
[Route("api/matches")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly MatchingEngine _matchingEngine;

    public MatchesController(MatchingEngine matchingEngine)
    {
        _matchingEngine = matchingEngine;
    }

    [HttpGet("seeker/{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Seeker))]
    public async Task<ActionResult<IEnumerable<Match>>> GetForSeeker(Guid id)
    {
        var matches = await _matchingEngine.TopMatchesForSeeker(id);
        return matches.ToList();
    }

    [HttpGet("owner/{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Owner))]
    public async Task<ActionResult<IEnumerable<Match>>> GetForOwner(Guid id)
    {
        var matches = await _matchingEngine.TopSeekersForOwner(id);
        return matches.ToList();
    }

    [HttpGet("score")]
    public async Task<ActionResult<object>> Score([FromQuery] Guid seekerId, [FromQuery] Guid roomId)
    {
        var result = await _matchingEngine.ScoreSeekerToRoom(seekerId, roomId);
        return new { Score = result.Score, Rationale = result.Rationale };
    }
}
