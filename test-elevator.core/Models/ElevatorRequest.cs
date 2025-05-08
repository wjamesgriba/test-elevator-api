using test_elevator.core.Enums;

namespace test_elevator.core.Models
{
    /// <summary>
    /// Represents a request for elevator service
    /// </summary>
    public class ElevatorRequest
    {
        public int SourceFloor { get; set; }
        public int DestinationFloor { get; set; }
        public Direction Direction { get; set; }
        public DateTime RequestTime { get; set; } = DateTime.UtcNow;
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public int? AssignedElevatorId { get; set; }

        /// <summary>
        /// Validates if the request is valid based on floor numbers and direction
        /// </summary>
        public bool IsValid()
        {
            if (SourceFloor < 1 || SourceFloor > 10 || DestinationFloor < 1 || DestinationFloor > 10)
            {
                Direction = Direction.None;
                Status = RequestStatus.Cancelled;
                return false;
            }

            if (SourceFloor == DestinationFloor)
            {
                Direction = Direction.None;
                Status = RequestStatus.Cancelled;
                return false;
            }

            var expectedDirection = SourceFloor < DestinationFloor ? Direction.Up : Direction.Down;
            return Direction == expectedDirection;
        }
    }
}
