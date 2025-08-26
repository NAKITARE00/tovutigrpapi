using Microsoft.AspNetCore.Mvc;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Repositories;


namespace tovutigrpapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyticsApiController : ControllerBase
    {
        private readonly IGadgets _gadgets;
        private readonly IIssues _issues;
        private readonly IClients _clients;
        private readonly IStations _stations;
        private readonly ISparePart _spareParts;

        public AnalyticsApiController(IGadgets gadgets,
            IIssues issues,
            IClients clients,
            IStations stations,
            ISparePart spareParts)
        {
            _gadgets = gadgets;
            _issues = issues;
            _clients = clients;
            _stations = stations;
            _spareParts = spareParts;
        }

        [HttpGet("data")]
        public IActionResult Get()
        {
            return Ok(new
            {
                Gadgets = _gadgets.GetAllGadgetsAnalytics(),
                Issues = _issues.GetAllIssuesAnalytics(),
                Spareparts = _spareParts.GetAllSpareParts(),
                Stations = _stations.GetAllStationsAnalytyics(),
                Clients = _clients.GetAllClients()
            });
        }
    }

}
