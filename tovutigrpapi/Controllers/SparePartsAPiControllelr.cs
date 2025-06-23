using Microsoft.AspNetCore.Mvc;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;

namespace tovutigrpapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SparePartsApiController : Controller
    {
        private readonly ISparePart _sparePartService;

        public SparePartsApiController(ISparePart sparePartService)
        {
            _sparePartService = sparePartService;
        }

        [HttpGet("GetAllSpareParts")]
        public async Task<IActionResult> GetAllSpareParts()
        {
            try
            {
                var parts = await _sparePartService.GetAllSpareParts();
                return Ok(parts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get-single-sparepart/{id}")]
        public async Task<IActionResult> GetSingleSparePart(int id)
        {
            try
            {
                var part = await _sparePartService.GetSingleSparePart(id);
                if (part == null)
                {
                    return NotFound($"Spare part with ID {id} not found.");
                }

                return Ok(part);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddSparePart")]
        public async Task<IActionResult> AddSparePart([FromBody] SparePart part)
        {
            if (part == null)
            {
                return BadRequest("Spare part data is null.");
            }

            try
            {
                string result = await _sparePartService.AddSparePart(part);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
