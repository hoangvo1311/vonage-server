using Microsoft.AspNetCore.Mvc;
using OpenTokSDK;
using Vonage;
using Vonage.Common.Monads;
using Vonage.Cryptography;
using Vonage.Request;
using Vonage.Video.Archives.CreateArchive;
using Vonage.Video.Authentication;
using Vonage.Video.Sessions.CreateSession;
using Archive = Vonage.Video.Archives.Archive;
using MediaMode = Vonage.Video.Sessions.MediaMode;

namespace VonageServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VonageController : ControllerBase
    {

        private static Dictionary<string, string>? _sessions;
        private const int ApiKey = 47881851;
        private const string Secret = "1c1d2ad8c4749bc9a22a196f6f3e2d1c724e4a32";
        public VonageController()
        {
            _sessions ??= new Dictionary<string, string>();
        }

        [HttpGet("session/{room}")]
        public async Task<IActionResult> GetSession(string room)
        {
            try
            {
                var opentok = new OpenTok(47881851, Secret);

                var sessionId = _sessions!.TryGetValue(room, out var storedSessionId) ? storedSessionId :
                    (await opentok.CreateSessionAsync()).Id;

                var token = opentok.GenerateToken(sessionId);
                var data = new OpenTokData()
                {
                    SessionId = sessionId,
                    Token = token,
                    ApiKey = ApiKey
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

        //[HttpGet("archive/{sessionId}")]
        //public async Task<IActionResult> StartArchiveAsync(string sessionId)
        //{
        //    var credentials = Credentials.FromAppIdAndPrivateKeyPath("098683eb-10d7-42fa-ac09-0a241a31c28a", "private.key");
        //    var client = new VonageClient(credentials);
        //    var videoClient = client.VideoClient;

        //    Result<CreateArchiveRequest> request = CreateArchiveRequest.Build()
        //        .WithApplicationId(Guid.Parse(_apiKey))
        //        .WithSessionId(sessionId)
        //        .Create();


        //    Result<Archive> response = await videoClient.ArchiveClient.CreateArchiveAsync(request);
        //    var archive = response.GetSuccessUnsafe();
        //    return Ok(archive);
        //}

    }

    public class OpenTokData
    {
        public string SessionId { get; set; }
        public string Token { get; set; }
        public int ApiKey { get; set; }
    }
}
