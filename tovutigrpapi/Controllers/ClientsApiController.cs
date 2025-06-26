using Microsoft.AspNetCore.Mvc;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;

namespace tovutigrpapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientsApiController : Controller
    {
        private readonly IClients _clientsService;

        public ClientsApiController(IClients clientsService)
        {
            _clientsService = clientsService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _clientsService.GetAllClients();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            var client = await _clientsService.GetSingleClient(id);
            if (client == null)
                return NotFound($"Client with ID {id} not found.");
            return Ok(client);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddClient([FromBody] Client client)
        {
            if (client == null)
                return BadRequest("Client is null");

            var result = await _clientsService.AddClient(client);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateClient([FromBody] Client client)
        {
            if (client == null || client.Id == 0)
                return BadRequest("Invalid client update data");

            var result = await _clientsService.UpdateClient(client);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var result = await _clientsService.DeleteClient(id);
            return Ok(result);
        }
    }
}
