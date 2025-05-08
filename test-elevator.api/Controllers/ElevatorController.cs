using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using test_elevator.core.DTOs;
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
            => _elevatorService = elevatorService;

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
        public IActionResult RequestElevator([FromBody] CreateElevatorRequest elevatorRequest)
        {
            if (elevatorRequest == null)
                return BadRequest("Request cannot be null");

            _elevatorService.RequestElevator(elevatorRequest.SourceFloor, elevatorRequest.DestinationFloor, elevatorRequest.Direction);
            return Ok("Request submitted successfully");
        }

        /// <summary>
        /// Starts the elevator simulation
        /// </summary>
        [HttpPost("start-simulation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult StartSimulation()
        {
            _elevatorService.StartSimulation();
            return Ok("Elevator simulation started");
        }

        /// <summary>
        /// Stops the elevator simulation
        /// </summary>
        [HttpPost("stop-simulation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult StopSimulation()
        {
            _elevatorService.StopSimulation();
            return Ok("Elevator simulation stopped");
        }
    }
}
