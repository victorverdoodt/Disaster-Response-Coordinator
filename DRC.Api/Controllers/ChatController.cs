using DRC.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DRC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("Conversation")]
        public async Task<IActionResult> Conversation([FromBody] string message, [FromQuery] Guid? guid = null) 
        {
            var response = await _chatService.SendMessage(guid, message);
            return Ok(new {Guid = response.guid, Response = response.message } );
        }
    }
}
