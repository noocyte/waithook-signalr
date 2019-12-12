using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace noocyte.Waithook_SignalR.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WaithookController : ControllerBase
    {
        private readonly IHubContext<WaithookHub> _hubContext;

        public WaithookController(IHubContext<WaithookHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        [HttpPost("{unique_name}")]
        public async Task<IActionResult> PostIt(string unique_name, CancellationToken cancellationToken)
        {
            if (unique_name is null) throw new ArgumentNullException(nameof(unique_name));

            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var body = await reader.ReadToEndAsync();
                var message = new WaithookMessage(Request, body);
                var content = JsonSerializer.Serialize(message);
                await _hubContext.Clients.Client(unique_name).SendAsync("Hook", content, cancellationToken);
                return Ok();
            }
        }
    }

    public class WaithookHub : Hub
    {
    }

    public class WaithookMessage
    {
        public WaithookMessage(HttpRequest request, string body)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            Body = body;
            Method = request.Method;
            Url = request.Path;
            Headers = request.Headers.ToDictionary(k => k.Key, v => v.Value.ToString());
        }

        public string Method { get; }
        public string Url { get; }
        public Dictionary<string, string> Headers { get; }
        public string Body { get; }
    }
}
