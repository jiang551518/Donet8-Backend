using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;


namespace Excel.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly string apiKey;
        private readonly IConfiguration _configuration;
        public AiController(IConfiguration configuration)
        {
            _configuration = configuration;
            apiKey = _configuration["DeepSeekApi"];
        }

        [HttpGet("stream")]
        [Produces("text/event-stream")]
        public async Task StreamAI([FromQuery] string prompt)
        {
            Response.ContentType = "text/event-stream; charset=utf-8";
            if (string.IsNullOrWhiteSpace(prompt))
            {
                await HttpContext.Response.WriteAsync("data: 请提供 prompt 参数。\n\n");
                await Response.Body.FlushAsync();
                return;
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var systemPrompt = "你是一个幽默风趣的旅游向导，总是用轻松口吻推荐景点。";

            var requestData = new
            {
                model = "deepseek-chat",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },   // 自定义角色设定
                    new { role = "user",   content = prompt }          // 用户提问
                },
                temperature = 0.7,
                max_tokens = 500,
                stream = true
            };
            var json = JsonConvert.SerializeObject(requestData);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.deepseek.com/chat/completions")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line) || !line.StartsWith("data:")) continue;
                var payload = line.Substring(5).Trim();
                if (payload == "[DONE]") break;

                await Response.WriteAsync($"data: {payload}\n\n");
                await Response.Body.FlushAsync();
            }
        }


    }
}
