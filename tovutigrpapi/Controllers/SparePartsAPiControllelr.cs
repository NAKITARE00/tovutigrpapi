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
        public async Task<IActionResult> GetAllSpareParts(int staff_id)
        {
            try
            {
                var parts = await _sparePartService.GetAllSpareParts(staff_id);
                return Ok(parts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get-single-sparepart/{id}")]
        public async Task<IActionResult> GetSingleSparePart(int id, int staff_id)
        {
            try
            {
                var part = await _sparePartService.GetSingleSparePart(id, staff_id);
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
        public async Task<IActionResult> AddSparePart([FromBody] SparePart part, int staff_id)
        {
            if (part == null)
            {
                return BadRequest("Spare part data is null.");
            }

            try
            {
                string result = await _sparePartService.AddSparePart(part, staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateSparePart")]
        public async Task<IActionResult> UpdateSparePart([FromBody] SparePart part, int staff_id)
        {
            if (part == null || part.Id <= 0)
            {
                return BadRequest("Invalid spare part data.");
            }

            try
            {
                string result = await _sparePartService.UpdateSparePart(part, staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteSparePart/{id}")]
        public async Task<IActionResult> DeleteSparePart(int id, int staff_id)
        {
            try
            {
                string result = await _sparePartService.DeleteSparePart(id, staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

