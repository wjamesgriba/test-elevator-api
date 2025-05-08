using Microsoft.Extensions.Logging;
using test_elevator.core.Enums;
using test_elevator.core.Interfaces;
using test_elevator.core.Models;

namespace test_elevator.core.Services
{
    /// <summary>
    /// Service responsible for managing elevator operations
    /// </summary>
    public class ElevatorService : IElevatorService
    {
        private readonly List<Elevator> _elevators;
        private readonly ILogger<ElevatorService> _logger;
        private readonly Random _random = new Random();
        private readonly object _lockObject = new object();
        private bool _isSimulationStarted;

        public ElevatorService(ILogger<ElevatorService> logger, bool startSimulation = false)
        {
            _logger = logger;
            _elevators = Enumerable.Range(1, 4).Select(id => new Elevator { Id = id }).ToList();
            if (startSimulation)
            {
                StartSimulation();
            }
        }

        public List<ElevatorStatus> GetElevatorStatuses() =>
            _elevators.Select(e => new ElevatorStatus
            {
                ElevatorId = e.Id,
                CurrentFloor = e.CurrentFloor,
                CurrentDirection = e.CurrentDirection,
                State = e.State,
                RequestCount = e.Requests.Count
            }).ToList();

        public void RequestElevator(int sourceFloor, int destinationFloor, Direction direction)
        {
            // Validate floor ranges
            if (sourceFloor < 1 || sourceFloor > 10 || destinationFloor < 1 || destinationFloor > 10)
            {
                _logger.LogWarning($"Invalid request: Floor numbers out of range. Source: {sourceFloor}, Destination: {destinationFloor}");
                return;
            }

            // Validate source and destination are different
            if (sourceFloor == destinationFloor)
            {
                _logger.LogWarning($"Invalid request: Source and destination floors are the same. Floor: {sourceFloor}");
                return;
            }

            // Validate direction matches floor movement
            var expectedDirection = sourceFloor < destinationFloor ? Direction.Up : Direction.Down;
            if (direction != expectedDirection)
            {
                _logger.LogWarning($"Invalid request: Direction mismatch. Expected: {expectedDirection}, Got: {direction}");
                return;
            }

            var request = new ElevatorRequest
            {
                SourceFloor = sourceFloor,
                DestinationFloor = destinationFloor,
                Direction = direction
            };


            var elevator = AssignElevator(request);
            if (elevator != null)
            {
                request.AssignedElevatorId = elevator.Id;
                elevator.AddRequest(request);
                _logger.LogInformation($"Request assigned to Elevator {elevator.Id}: " +
                    $"Floor {sourceFloor} to {destinationFloor}, Direction: {direction}");
            }
            else
            {
                _logger.LogWarning("No available elevator found for the request");
            }
        }

        private Elevator AssignElevator(ElevatorRequest request)
        {
            lock (_lockObject)
            {
                // First try to find an idle elevator
                var idleElevator = _elevators.FirstOrDefault(e => e.State == ElevatorState.Idle);
                if (idleElevator != null)
                    return idleElevator;

                // Then try to find an elevator going in the same direction
                var sameDirectionElevator = _elevators
                    .Where(e => e.CurrentDirection == request.Direction)
                    .OrderBy(e => Math.Abs(e.CurrentFloor - request.SourceFloor))
                    .FirstOrDefault();

                if (sameDirectionElevator != null)
                    return sameDirectionElevator;

                // Finally, find the closest elevator
                return _elevators
                    .OrderBy(e => Math.Abs(e.CurrentFloor - request.SourceFloor))
                    .FirstOrDefault();
            }
        }

        public void StartSimulation()
        {
            if (_isSimulationStarted)
            {
                return;
            }

            _isSimulationStarted = true;
            foreach (var elevator in _elevators)
            {
                Task.Run(() => ProcessElevator(elevator));
            }

            // Random request generator
            Task.Run(async () =>
            {
                while (_isSimulationStarted)
                {
                    int sourceFloor = _random.Next(1, 11);
                    int destinationFloor;
                    do
                    {
                        destinationFloor = _random.Next(1, 11);
                    } while (destinationFloor == sourceFloor);

                    var direction = sourceFloor < destinationFloor ? Direction.Up : Direction.Down;
                    RequestElevator(sourceFloor, destinationFloor, direction);
                    await Task.Delay(15000); // 15s between requests
                }
            });
        }

        public void StopSimulation()
        {
            _isSimulationStarted = false;
        }

        private async Task ProcessElevator(Elevator elevator)
        {
            while (_isSimulationStarted)
            {
                try
                {
                    if (elevator.Requests.TryPeek(out var currentRequest))
                    {
                        _logger.LogInformation($"Elevator {elevator.Id} processing request: Floor {currentRequest.SourceFloor} to {currentRequest.DestinationFloor}");

                        // Move to source floor if not there
                        if (elevator.CurrentFloor != currentRequest.SourceFloor)
                        {
                            _logger.LogInformation($"Elevator {elevator.Id} moving to source floor {currentRequest.SourceFloor}");
                            await MoveElevator(elevator, currentRequest.SourceFloor);
                            await Task.Delay(100); // Small delay to ensure state update
                        }

                        // Open doors at source floor
                        _logger.LogInformation($"Elevator {elevator.Id} opening doors at source floor {currentRequest.SourceFloor}");
                        await HandleDoorOperations(elevator, true);
                        await Task.Delay(100); // Small delay to ensure state update

                        // Move to destination floor
                        if (elevator.CurrentFloor != currentRequest.DestinationFloor)
                        {
                            _logger.LogInformation($"Elevator {elevator.Id} moving to destination floor {currentRequest.DestinationFloor}");
                            await MoveElevator(elevator, currentRequest.DestinationFloor);
                            await Task.Delay(100); // Small delay to ensure state update
                        }

                        // Open doors at destination floor
                        _logger.LogInformation($"Elevator {elevator.Id} opening doors at destination floor {currentRequest.DestinationFloor}");
                        await HandleDoorOperations(elevator, true);
                        await Task.Delay(100); // Small delay to ensure state update

                        // Remove completed request
                        elevator.DequeueNextRequest();
                        currentRequest.Status = RequestStatus.Completed;

                        // Update elevator state
                        if (!elevator.Requests.Any())
                        {
                            elevator.State = ElevatorState.Idle;
                            elevator.CurrentDirection = Direction.None;
                            _logger.LogInformation($"Elevator {elevator.Id} returned to idle state");
                        }
                    }
                    else
                    {
                        await Task.Delay(1000); // Check every second
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing elevator {elevator.Id}");
                    await Task.Delay(1000);
                }
            }
        }

        private async Task MoveElevator(Elevator elevator, int targetFloor)
        {
            elevator.State = ElevatorState.Moving;
            elevator.CurrentDirection = elevator.CurrentFloor < targetFloor ? Direction.Up : Direction.Down;

            while (elevator.CurrentFloor != targetFloor && _isSimulationStarted)
            {
                await Task.Delay(Elevator.FLOOR_TRAVEL_TIME_MS);

                if (elevator.CurrentDirection == Direction.Up)
                    elevator.CurrentFloor++;
                else
                    elevator.CurrentFloor--;

                _logger.LogInformation($"Elevator {elevator.Id} at floor {elevator.CurrentFloor}");
            }

            // Ensure we reach the target floor
            if (elevator.CurrentFloor != targetFloor)
            {
                elevator.CurrentFloor = targetFloor;
                _logger.LogInformation($"Elevator {elevator.Id} reached target floor {targetFloor}");
            }
        }

        private async Task HandleDoorOperations(Elevator elevator, bool isOpening)
        {
            elevator.State = isOpening ? ElevatorState.DoorOpen : ElevatorState.DoorClosing;
            await Task.Delay(Elevator.DOOR_OPERATION_TIME_MS);
        }
    }
}
