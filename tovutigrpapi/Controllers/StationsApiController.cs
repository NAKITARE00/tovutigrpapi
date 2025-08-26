using Microsoft.AspNetCore.Mvc;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;
using tovutigrpapi.Services;

namespace tovutigrpapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StationsApiController : Controller
    {
        private readonly IStations stations;
        private readonly AuthorizationService authorizationService;

        public StationsApiController(IStations stations, AuthorizationService authorizationService)
        {
            this.stations = stations;
            this.authorizationService = authorizationService;
        }

        [HttpGet("GetAllStations")]
        public async Task<IActionResult> GetAllStations(int staff_id)
        {
            try
            {
                var result = await stations.GetAllStations(staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddStations")]
        public async Task<IActionResult> AddStation([FromBody] Stations station, int staff_id)
        {
            if (station == null)
                return BadRequest("Station data is null.");

            try
            {
                var result = await stations.AddStation(station, staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("stations-by-id/{stationId}")]
        public async Task<IActionResult> GetStationById(int stationId, int staff_id)
        {
            var result = await stations.GetSingleStation(stationId, staff_id);
            if (result == null || !result.Any())
                return NotFound($"No station found for ID {stationId}");
            var station = result.First();
            return Ok(result);
        }



        [HttpGet("client/{clientId}/stations")]
        public async Task<IActionResult> GetStationsByClientId(int clientId, int staff_id)
        {
            try
            {
                var result = await stations.GetStationsByClientId(clientId, staff_id);
                if (result == null || !result.Any())
                    return NotFound($"No stations found for client ID {clientId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateStation")]
        public async Task<IActionResult> UpdateStation([FromBody] Stations station, int staff_id)
        {
            if (station == null || station.Id <= 0)
                return BadRequest("Invalid station data.");

            try
            {
                var result = await stations.UpdateStation(station, staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteStation/{id}")]
        public async Task<IActionResult> DeleteStation(int id, int staff_id)
        {
            try
            {
                var station = (await stations.GetSingleStation(id, staff_id))?.FirstOrDefault();
                if (station == null)
                    return NotFound($"No station found with ID {id}");

                var result = await stations.DeleteStation(id, staff_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

