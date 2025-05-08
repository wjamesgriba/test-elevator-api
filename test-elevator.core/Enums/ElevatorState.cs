using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_elevator.core.Enums
{
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
}
