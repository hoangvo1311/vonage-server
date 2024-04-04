using Microsoft.AspNetCore.Mvc;
using OpenTokSDK;

namespace VonageServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VonageController : ControllerBase
    {
        private readonly OpenTok _opentok = new(Constants.ApiKey, Constants.Secret);

        [HttpGet("token/{sessionId}")]
        public async Task<IActionResult> GetToken(string sessionId)
        {
            try
            {
                //var sessionId = _sessions!.TryGetValue(room, out var storedSessionId) ? storedSessionId :
                //    (await _opentok!.CreateSessionAsync(mediaMode: MediaMode.ROUTED)).Id;
                double in30Days = (DateTime.UtcNow.Add(TimeSpan.FromDays(30)).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var token = _opentok!.GenerateToken(sessionId, expireTime: in30Days);
                var data = new OpenTokData()
                {
                    SessionId = sessionId,
                    Token = token,
                    ApiKey = Constants.ApiKey
                };

                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("session")]
        public async Task<IActionResult> GenerateSession()
        {
            try
            {
                //var sessionId = _sessions!.TryGetValue(room, out var storedSessionId) ? storedSessionId :
                //    (await _opentok!.CreateSessionAsync(mediaMode: MediaMode.ROUTED)).Id;

                var sessionId = (await _opentok!.CreateSessionAsync(mediaMode: MediaMode.ROUTED)).Id;
                return Ok(sessionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }

    public class OpenTokData
    {
        public string SessionId { get; set; }
        public string Token { get; set; }
        public int ApiKey { get; set; }
    }
}
