using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using test_elevator.core.Models;
using test_elevator.core.Services;

namespace test_elevator.tests
{
    [TestFixture]
    public class ElevatorServiceTests
    {
        private Mock<ILogger<ElevatorService>> _loggerMock;
        private ElevatorService _elevatorService;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ElevatorService>>();
            _elevatorService = new ElevatorService(_loggerMock.Object, startSimulation: false);
        }

        [TearDown]
        public void TearDown()
        {
            _elevatorService.StopSimulation();
        }

        [Test]
        public void GetElevatorStatuses_ShouldReturnAllElevators()
        {
            // Act
            var statuses = _elevatorService.GetElevatorStatuses();

            // Assert
            Assert.That(statuses.Count, Is.EqualTo(4));
            Assert.That(statuses.All(s => s.CurrentFloor == 1));
            Assert.That(statuses.All(s => s.CurrentDirection == Direction.None));
            Assert.That(statuses.All(s => s.State == ElevatorState.Idle));
            Assert.That(statuses.All(s => s.RequestCount == 0));
        }

        [Test]
        public void RequestElevator_WithInvalidFloor_ShouldNotAssignElevator()
        {
            // Act
            _elevatorService.RequestElevator(0, 5, Direction.Up);

            // Assert
            var statuses = _elevatorService.GetElevatorStatuses();
            Assert.That(statuses.All(s => s.RequestCount == 0));
        }

        [Test]
        public void RequestElevator_WithSameSourceAndDestination_ShouldNotAssignElevator()
        {
            // Act
            _elevatorService.RequestElevator(5, 5, Direction.Up);

            // Assert
            var statuses = _elevatorService.GetElevatorStatuses();
            Assert.That(statuses.All(s => s.RequestCount == 0));
        }

        [Test]
        public void RequestElevator_WithInvalidDirection_ShouldNotAssignElevator()
        {
            // Act
            _elevatorService.RequestElevator(5, 3, Direction.Up);

            // Assert
            var statuses = _elevatorService.GetElevatorStatuses();
            Assert.That(statuses.All(s => s.RequestCount == 0));
        }

        [Test]
        public async Task RequestElevator_WithValidRequest_ShouldAssignElevator()
        {
            // Act
            _elevatorService.RequestElevator(1, 5, Direction.Up);

            // Assert
            var statuses = _elevatorService.GetElevatorStatuses();
            Assert.That(statuses.Count(s => s.RequestCount == 1), Is.EqualTo(1));
            Assert.That(statuses.Count(s => s.RequestCount == 0), Is.EqualTo(3));
        }

        [Test]
        public async Task RequestElevator_MultipleRequests_ShouldDistributeAmongElevators()
        {
            // Act
            _elevatorService.RequestElevator(1, 5, Direction.Up);
            _elevatorService.RequestElevator(2, 6, Direction.Up);
            _elevatorService.RequestElevator(3, 7, Direction.Up);
            _elevatorService.RequestElevator(4, 8, Direction.Up);

            // Assert
            var statuses = _elevatorService.GetElevatorStatuses();
            Assert.That(statuses.All(s => s.RequestCount == 1));
        }

        [Test]
        public async Task Elevator_Moves_Floor_By_Floor()
        {
            // Arrange
            int sourceFloor = 1;
            int destinationFloor = 3;
            var direction = Direction.Up;

            // Calculate expected time
            int floorDifference = Math.Abs(destinationFloor - sourceFloor);
            int expectedTravelTime = floorDifference * Elevator.FLOOR_TRAVEL_TIME_MS;
            int expectedDoorTime = Elevator.DOOR_OPERATION_TIME_MS * 2; // Doors open twice
            int totalExpectedTime = expectedTravelTime + expectedDoorTime;
            int bufferTime = 5000; // 5 second buffer

            // Start the simulation
            _elevatorService.StartSimulation();

            // Act
            _elevatorService.RequestElevator(sourceFloor, destinationFloor, direction);

            // Wait for elevator to reach destination with progress checks
            var startTime = DateTime.UtcNow;
            var timeout = TimeSpan.FromMilliseconds(totalExpectedTime + bufferTime);
            var checkInterval = TimeSpan.FromSeconds(1);

            bool elevatorReachedDestination = false;
            ElevatorStatus finalElevator = null;

            while (DateTime.UtcNow - startTime < timeout && !elevatorReachedDestination)
            {
                var statuses = _elevatorService.GetElevatorStatuses();
                var movingElevator = statuses.FirstOrDefault(s => s.State == ElevatorState.Moving || s.State == ElevatorState.DoorOpen);

                if (movingElevator != null)
                {
                    TestContext.WriteLine($"Elevator {movingElevator.ElevatorId} at floor {movingElevator.CurrentFloor}, State: {movingElevator.State}, Direction: {movingElevator.CurrentDirection}");

                    if (movingElevator.CurrentFloor == destinationFloor)
                    {
                        elevatorReachedDestination = true;
                        finalElevator = movingElevator;
                    }
                }

                if (!elevatorReachedDestination)
                {
                    await Task.Delay(checkInterval);
                }
            }

            // If we get here, either the elevator reached the destination or timed out
            if (!elevatorReachedDestination)
            {
                var statuses = _elevatorService.GetElevatorStatuses();
                finalElevator = statuses.FirstOrDefault(s => s.State == ElevatorState.Moving || s.State == ElevatorState.DoorOpen);
                TestContext.WriteLine($"Final elevator state - Floor: {finalElevator?.CurrentFloor}, State: {finalElevator?.State}, Direction: {finalElevator?.CurrentDirection}");
            }

            Assert.That(finalElevator, Is.Not.Null, "An elevator should be in motion or at doors");
            Assert.That(finalElevator.CurrentFloor, Is.EqualTo(destinationFloor),
                $"Elevator should reach destination floor. Current floor: {finalElevator.CurrentFloor}, Expected: {destinationFloor}");
        }
    }
}