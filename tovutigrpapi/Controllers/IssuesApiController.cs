using Microsoft.AspNetCore.Mvc;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;

namespace tovutigrpapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IssuesApiController : Controller
    {
        private readonly IIssues _issuesService;

        public IssuesApiController(IIssues issuesService)
        {
            _issuesService = issuesService;
        }

        [HttpGet("GetAllIssues")]
        public async Task<IActionResult> GetAllIssues(int staff_id)
        {
            try
            {
                var issuesList = await _issuesService.GetAllIssues(staff_id);
                return Ok(issuesList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddIssue")]
        public async Task<IActionResult> AddIssue([FromBody] Issues issue, int staff_id)
        {
            if (issue == null)
                return BadRequest("Issue data is null.");

            try
            {
                string result = await _issuesService.AddIssue(issue, staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("single-issue/{issueId}")]
        public async Task<IActionResult> GetSingleIssue(int issueId, int staff_id)
        {
            try
            {
                var result = await _issuesService.GetSingleIssue(issueId, staff_id);
                if (result == null)
                    return NotFound($"Issue with ID {issueId} not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateIssue")]
        public async Task<IActionResult> UpdateIssue([FromBody] Issues issue, int staff_id)
        {
            if (issue == null || issue.Id == 0)
                return BadRequest("Invalid issue data.");

            try
            {
                var result = await _issuesService.UpdateIssue(issue, staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteIssue/{id}")]
        public async Task<IActionResult> DeleteIssue(int id, int staff_id)
        {
            try
            {
                var result = await _issuesService.DeleteIssue(id, staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
