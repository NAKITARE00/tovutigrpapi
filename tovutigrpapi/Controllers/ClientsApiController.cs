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
        public async Task<IActionResult> GetAllClients(int staff_id)
        {
            var clients = await _clientsService.GetAllClients(staff_id);
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(int id, int staff_id)
        {
            var client = await _clientsService.GetSingleClient(id, staff_id);
            if (client == null)
                return NotFound($"Client with ID {id} not found.");
            return Ok(client);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddClient([FromBody] Client client, int staff_id)
        {
            if (client == null)
                return BadRequest("Client is null");

            var result = await _clientsService.AddClient(client, staff_id);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateClient([FromBody] Client client, int staff_id)
        {
            if (client == null || client.Id == 0)
                return BadRequest("Invalid client update data");

            var result = await _clientsService.UpdateClient(client, staff_id);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteClient(int id, int staff_id)
        {
            var result = await _clientsService.DeleteClient(id, staff_id);
            return Ok(result);
        }
    }
}
