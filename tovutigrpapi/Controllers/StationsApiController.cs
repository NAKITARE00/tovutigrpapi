using Microsoft.AspNetCore.Mvc;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;
using System.Collections.Generic;

namespace tovutigrpapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StationsApiController : Controller
    {
        private readonly IStations stations;
        public StationsApiController(IStations stations)
        {
            this.stations = stations;
        }

        [HttpGet("GetAllStations")]
        public async Task<IActionResult> GetAllStations()
        {
            StationsApiController controller = this;
            try {
                IEnumerable<Stations> cars = await controller.stations.GetAllStations();
                return (IActionResult)controller.Ok((object) cars);
            }
            catch(Exception ex)
            {
                return (IActionResult) controller.StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddStations")]
        public async Task<IActionResult> AddCar([FromBody] Stations station)
        {
            if (station == null)
            {
                return BadRequest("Station data is null.");
            }

            try
            {
                string result = await stations.AddStation(station);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("stations-by-id/{stationId}")]
        public async Task<IActionResult> GetCarsByModelId(int stationId)
        {
            var result = await stations.GetSingleStation(stationId);

            if (result == null || !result.Any())
                return NotFound($"No station found for station ID {stationId}");

            return Ok(result);
        }

    }
}
