using test_elevator.core.Enums;

namespace test_elevator.core.Models
{
    /// <summary>
    /// Represents the current status of an elevator
    /// </summary>
    public class ElevatorStatus
    {
        public int ElevatorId { get; set; }
        public int CurrentFloor { get; set; }
        public Direction CurrentDirection { get; set; }
        public ElevatorState State { get; set; }
        public int RequestCount { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
