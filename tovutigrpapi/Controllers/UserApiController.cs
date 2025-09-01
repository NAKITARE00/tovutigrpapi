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

        [HttpGet("get-single-user/{id}")]
        public async Task<IActionResult> GetSingleUser(int id)
        {
            try
            {
                var user = await _users.GetSingleUser(id);
                if (user == null)
                    return NotFound($"User with ID {id} not found.");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get-by-email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _users.GetUserByEmail(email);
                if (user == null)
                    return NotFound($"User with email {email} not found.");
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] Users user)
        {
            if (user == null || user.Id <= 0)
                return BadRequest("Invalid user data.");

            try
            {
                string result = await _users.UpdateUser(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                string result = await _users.DeleteUser(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                IEnumerable<Role> roles = await _users.GetAllRoles();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
