using test_elevator.core.DTOs;
using test_elevator.core.Enums;
using test_elevator.core.Models;

namespace test_elevator.core.Interfaces
{
    /// <summary>
    /// Interface for elevator service operations
    /// </summary>
    public interface IElevatorService
    {
        /// <summary>
        /// Gets the current status of all elevators
        /// </summary>
        List<ElevatorStatus> GetElevatorStatuses();

        /// <summary>
        /// Requests an elevator to pick up passengers
        /// </summary>
        /// <param name="sourceFloor">The floor where passengers are waiting</param>
        /// <param name="destinationFloor">The floor where passengers want to go</param>
        /// <param name="direction">The direction of travel</param>
        void RequestElevator(int sourceFloor, int destinationFloor, Direction direction);

        /// <summary>
        /// Starts the elevator simulation
        /// </summary>
        void StartSimulation();

        /// <summary>
        /// Stops the elevator simulation
        /// </summary>
        void StopSimulation();
    }
}
