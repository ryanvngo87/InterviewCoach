using interviewCoachAI.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace interviewCoachAI.Controllers
{
    [ApiController]
    [Route("api/speech")]
    public class SpeechController : ControllerBase
    {
        private readonly AzureSpeechOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public SpeechController(IOptions<AzureSpeechOptions> options, IHttpClientFactory httpClientFactory)
        {
            this._options = options.Value;
            this._httpClientFactory = httpClientFactory;
        }

        [HttpGet("token")]
        public async Task<IActionResult> GetToken()
        {
            var client = _httpClientFactory.CreateClient();

            var tokenURL = $"https://{_options.Region}.api.cognitive.microsoft.com/sts/v1.0/issueToken";

            using var request = new HttpRequestMessage(HttpMethod.Post, tokenURL);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _options.Key);

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to get speech token.");
            }

            var token = await response.Content.ReadAsStringAsync();
            return Ok(new { token, region = _options.Region });
        }
    }
}
