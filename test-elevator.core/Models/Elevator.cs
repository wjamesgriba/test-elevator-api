namespace test_elevator.core.Models
{
    /// <summary>
    /// Represents an elevator in the system
    /// </summary>
    public class Elevator
    {
        public int Id { get; set; }
        public int CurrentFloor { get; set; } = 1;
        public Direction CurrentDirection { get; set; } = Direction.None;
        public ElevatorState State { get; set; } = ElevatorState.Idle;
        public Queue<ElevatorRequest> Requests { get; set; } = new();
        public bool IsBusy => State != ElevatorState.Idle;

        // Constants for timing
        public const int FLOOR_TRAVEL_TIME_MS = 10000; // 10 seconds per floor
        public const int DOOR_OPERATION_TIME_MS = 10000; // 10 seconds for door operations

        /// <summary>
        /// Gets the next request in the queue without removing it
        /// </summary>
        public ElevatorRequest PeekNextRequest() => Requests.Peek();

        /// <summary>
        /// Removes and returns the next request from the queue
        /// </summary>
        public ElevatorRequest DequeueNextRequest() => Requests.Dequeue();

        /// <summary>
        /// Adds a new request to the elevator's queue
        /// </summary>
        public void AddRequest(ElevatorRequest request)
        {
            Requests.Enqueue(request);
            if (State == ElevatorState.Idle)
            {
                State = ElevatorState.Moving;
                CurrentDirection = request.Direction;
            }
        }
    }

    /// <summary>
    /// Represents the possible states of an elevator
    /// </summary>
    public enum ElevatorState
    {
        Idle,
        Moving,
        DoorOpen,
        DoorClosing
    }

    /// <summary>
    /// Represents the direction of elevator movement
    /// </summary>
    public enum Direction
    {
        None,
        Up,
        Down
    }
}
