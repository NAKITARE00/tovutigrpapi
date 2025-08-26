using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;
using tovutigrpapi.Repositories;

namespace tovutigrpapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GadgetsApiController : Controller
    {
        private readonly IGadgets gadgets;

        public GadgetsApiController(IGadgets gadgets)
        {
            this.gadgets = gadgets;
        }

        [HttpGet("GetAllGadgets")]
        public async Task<IActionResult> GetAllGadgets(int staff_id)
        {
            try
            {
                var gadgetsList = await gadgets.GetAllGadgets(staff_id);
                return Ok(gadgetsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddGadgets")]
        public async Task<IActionResult> AddGadget([FromBody] Gadgets gadget, int staff_id)
        {
            if (gadget == null)
                return BadRequest("Gadget data is null.");

            try
            {
                string result = await gadgets.AddGadget(gadget, staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("single-gadget/{gadgetId}")]
        public async Task<IActionResult> GetSingleGadget(int gadgetId, int staff_id)
        {
            var result = await gadgets.GetSingleGadget(gadgetId, staff_id);

            if (result == null)
                return NotFound($"Gadget with ID {gadgetId} not found.");

            return Ok(result);
        }

        [HttpPut("UpdateGadget")]
        public async Task<IActionResult> UpdateGadget([FromBody] Gadgets gadget, int staff_id)
        {
            if (gadget == null || gadget.Id == 0)
                return BadRequest("Invalid gadget data.");

            try
            {
                var result = await gadgets.UpdateGadget(gadget, staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("station/{stationId}/gadgets")]
        public async Task<IActionResult> GetGadgetsByStationId(int stationId, int staff_id)
        {
            try
            {
                var gadgetsList = await gadgets.GetGadgetsByStationId(stationId, staff_id);
                return Ok(gadgetsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteGadget/{id}")]
        public async Task<IActionResult> DeleteGadget(int id, int staff_id)
        {
            try
            {
                var result = await gadgets.DeleteGadget(id, staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("RestoreGadget/{id}")]
        public async Task<IActionResult> RestoreGadget(int id, int staff_id)
        {
            try
            {
                var result = await gadgets.RestoreGadget(id, staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddSparepart")]
        public async Task<IActionResult> AddSparepartToGadget(int gadgetId, int sparePartId, int staff_id)
        {
            try {
                var response = await gadgets.AddSparepart(gadgetId, sparePartId, staff_id);
                return Ok(new { message = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
    }
}
