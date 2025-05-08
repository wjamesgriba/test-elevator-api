using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using test_elevator.core.Interfaces;
using test_elevator.core.Models;

namespace test_elevator.api.Controllers
{
    /// <summary>
    /// Controller for managing elevator operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ElevatorController : ControllerBase
    {
        private readonly IElevatorService _elevatorService;

        public ElevatorController(IElevatorService elevatorService)
        {
            _elevatorService = elevatorService;
        }

        /// <summary>
        /// Gets the current status of all elevators
        /// </summary>
        [HttpGet("status")]
        [ProducesResponseType(typeof(List<ElevatorStatus>), StatusCodes.Status200OK)]
        public IActionResult GetStatuses()
        {
            return Ok(_elevatorService.GetElevatorStatuses());
        }

        /// <summary>
        /// Requests an elevator to pick up passengers
        /// </summary>
        [HttpPost("request")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult RequestElevator([FromBody] ElevatorRequest request)
        {
            if (!request.IsValid())
            {
                return BadRequest("Invalid request parameters");
            }

            _elevatorService.RequestElevator(request.SourceFloor, request.DestinationFloor, request.Direction);
            return Ok("Request submitted successfully");
        }
    }
}
