using test_elevator.core.Enums;

namespace test_elevator.core.DTOs
{
    public class CreateElevatorRequest
    {
        public int SourceFloor { get; set; }
        public int DestinationFloor { get; set; }
        public Direction Direction { get; set; }
    }
}
