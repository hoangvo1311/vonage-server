using Microsoft.AspNetCore.Mvc;
using OpenTokSDK;

namespace VonageServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VonageController : ControllerBase
    {
        private static Dictionary<string, string> _sessions;


        private readonly OpenTok _opentok = new(Constants.ApiKey, Constants.Secret);

        public VonageController()
        {
            _sessions ??= new Dictionary<string, string>();
        }

        [HttpGet("session/{room}")]
        public async Task<IActionResult> GetSession(string room)
        {
            try
            {
                var sessionId = _sessions!.TryGetValue(room, out var storedSessionId) ? storedSessionId :
                    (await _opentok!.CreateSessionAsync(mediaMode:MediaMode.ROUTED)).Id;

                double in30Days = (DateTime.UtcNow.Add(TimeSpan.FromDays(30)).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var token = _opentok!.GenerateToken(sessionId, expireTime: in30Days);
                var data = new OpenTokData()
                {
                    SessionId = sessionId,
                    Token = token,
                    ApiKey = Constants.ApiKey
                };

                _sessions[room] = sessionId;
                return Ok(data);
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
