using Microsoft.AspNetCore.Mvc;
using OpenTokSDK;

namespace VonageServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchiveController : ControllerBase
    {
        private readonly OpenTok _opentok = new(Constants.ApiKey, Constants.Secret);

        [HttpGet("start/{sessionId}")]
        public async Task<IActionResult> StartArchiveAsync(string sessionId)
        {
            try
            {
                var archive = await _opentok.StartArchiveAsync(sessionId);
                return Ok(archive);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("stop/{archiveId}")]
        public async Task<IActionResult> StopArchiveAsync(string archiveId)
        {
            try
            {
                var archive = await _opentok.StopArchiveAsync(archiveId);
                return Ok(archive);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{sessionId}")]
        public async Task<IActionResult> FetchArchiveListAsync(string sessionId)
        {
            try
            {
                var archives = await _opentok.ListArchivesAsync(sessionId: sessionId);
                foreach (var archive in archives)
                {
                    archive.Url =
                        $"https://hoangvonage.blob.core.windows.net/demo/{archive.PartnerId}/{archive.Id}/archive.mp4";
                }
                return Ok(archives);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
