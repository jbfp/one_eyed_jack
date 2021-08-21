using Microsoft.AspNetCore.Mvc;

namespace Sequence.ClientSideLogging
{
    [ApiController]
    public sealed class ClientSideLoggingController : ControllerBase
    {
        private readonly ILogger<ClientSideLoggingController> _logger;

        public ClientSideLoggingController(ILogger<ClientSideLoggingController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/logs")]
        public void Post([FromBody] LogModel model)
        {
            _logger.LogError("Client side error: {@Error}", model);
        }
    }

    public sealed record LogModel(int LineNumber, string? FileName, int ColumnNumber, string? Stack, string? Message);
}
