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
            try
            {
                IEnumerable<Stations> result = await stations.GetAllStations();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddStations")]
        public async Task<IActionResult> AddStation([FromBody] Stations station)
        {
            if (station == null)
                return BadRequest("Station data is null.");

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
        public async Task<IActionResult> GetStationById(int stationId)
        {
            var result = await stations.GetSingleStation(stationId);

            if (result == null || !result.Any())
                return NotFound($"No station found for ID {stationId}");

            return Ok(result);
        }

        [HttpGet("client/{clientId}/stations")]
        public async Task<IActionResult> GetStationsByClientId(int clientId)
        {
            var result = await stations.GetStationsByClientId(clientId);

            if (result == null || !result.Any())
                return NotFound($"No stations found for client ID {clientId}");

            return Ok(result);
        }


        [HttpPut("UpdateStation")]
        public async Task<IActionResult> UpdateStation([FromBody] Stations station)
        {
            if (station == null || station.Id <= 0)
                return BadRequest("Invalid station data.");

            try
            {
                string result = await stations.UpdateStation(station);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteStation/{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            try
            {
                string result = await stations.DeleteStation(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

