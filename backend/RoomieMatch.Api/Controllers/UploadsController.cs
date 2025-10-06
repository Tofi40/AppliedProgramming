using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoomieMatch.Infrastructure.Storage;

namespace RoomieMatch.Api.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize]
public class UploadsController : ControllerBase
{
    private readonly IFileStorage _fileStorage;

    public UploadsController(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    [HttpPost("images")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<object>> UploadImage(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required");
        }

        await using var stream = file.OpenReadStream();
        var url = await _fileStorage.SaveAsync(stream, file.ContentType, file.FileName, cancellationToken);
        return new { Url = url };
    }
}
