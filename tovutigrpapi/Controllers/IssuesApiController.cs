using Microsoft.AspNetCore.Mvc;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;
using System.Collections.Generic;

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
        public async Task<IActionResult> GetAllIssues()
        {
            try
            {
                IEnumerable<Issues> issuesList = await _issuesService.GetAllIssues();
                return Ok(issuesList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddIssue")]
        public async Task<IActionResult> AddIssue([FromBody] Issues issue)
        {
            if (issue == null)
            {
                return BadRequest("Issue data is null.");
            }

            try
            {
                string result = await _issuesService.AddIssue(issue);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("single-issue/{issueId}")]
        public async Task<IActionResult> GetSingleIssue(int issueId)
        {
            try
            {
                var result = await _issuesService.GetSingleIssue(issueId);
                if (result == null)
                {
                    return NotFound($"Issue with ID {issueId} not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

