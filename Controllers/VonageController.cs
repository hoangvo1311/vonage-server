using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vonage;
using Vonage.Request;
using Vonage.Video.Authentication;
using Vonage.Video.Sessions.CreateSession;
using Vonage.Video.Sessions;
using System.Reflection.Emit;
using Vonage.Video;

namespace VonageServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VonageController : ControllerBase
    {
        private static Dictionary<string, string> _sessions;
        private string _apiKey = "098683eb-10d7-42fa-ac09-0a241a31c28a";

        public VonageController()
        {
            _sessions ??= new Dictionary<string, string>();
        }

        [HttpGet("session/{room}")]
        public async Task<IActionResult> GetSession(string room)
        {
            try
            {
                var generator = new VideoTokenGenerator();
                var credentials = Credentials.FromAppIdAndPrivateKeyPath("098683eb-10d7-42fa-ac09-0a241a31c28a", "private.key");
                var client = new VonageClient(credentials);
                var videoClient = client.VideoClient;

                var sessionId = "";
                if (_sessions.ContainsKey(room))
                {
                    sessionId = _sessions[room];
                }
                else
                {
                    // Create a default session request
                    var createSessionRequest = CreateSessionRequest.Default;
                    var newSession = await videoClient.SessionClient.CreateSessionAsync(createSessionRequest);
                    sessionId = newSession.GetSuccessUnsafe().SessionId;
                }

                var claims = TokenAdditionalClaims.Parse(sessionId);

                var videoToken = generator.GenerateToken(credentials, claims).GetSuccessUnsafe().Token;

                var data = new OpenTokData()
                {
                    SessionId = sessionId,
                    Token = videoToken,
                    ApiKey = _apiKey
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
        public string ApiKey { get; set; }
    }
}
