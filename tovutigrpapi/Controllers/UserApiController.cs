using Microsoft.AspNetCore.Mvc;
using tovutigrpapi.Models;
using tovutigrpapi.Interfaces;
using System.Collections.Generic;

namespace tovutigrpapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserApiController : Controller
    {
        private readonly IUsers _users;

        public UserApiController(IUsers users)
        {
            _users = users;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                IEnumerable<Users> users = await _users.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] Users user)
        {
            if (user == null)
                return BadRequest("User data is null.");

            try
            {
                string result = await _users.AddUser(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

