using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;
using tovutigrpapi.Repositories;

namespace tovutigrpapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GadgetsApiController : Controller
    {
        private readonly Interfaces.IGadgets gadgets;
        public GadgetsApiController(Interfaces.IGadgets gadgets)
        {
            this.gadgets = gadgets;
        }

        [HttpGet("GetAllGadgets")]
        public async Task<IActionResult> GetAllGadgets()
        {
            GadgetsApiController controller = this;
            try
            {
                IEnumerable<Models.Gadgets> gadgetsList = (IEnumerable<Gadgets>)await controller.gadgets.GetAllGadgets();
                return (IActionResult)controller.Ok((object)gadgetsList);
            }
            catch (Exception ex)
            {
                return (IActionResult)controller.StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddGadgets")]
        public async Task<IActionResult> AddGadget([FromBody] Models.Gadgets gadget)
        {
            if (gadget == null)
            {
                return BadRequest("Sale data is null.");
            }
            try
            {
                string result = await gadgets.AddGadget(gadget);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("single-gadget/{gadgetId}")]
        public async Task<IActionResult> GetSingleGadget(int gadgetId)
        {
            var result = await gadgets.GetSingleGadget(gadgetId);

            if (result == null)
                return NotFound($"Sale with ID {gadgetId} not found.");

            return Ok(result);
        }

    }
}
