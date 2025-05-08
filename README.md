# Elevator Control System API

## Overview
This is a RESTful API for controlling a multi-elevator system. The system manages multiple elevators, handles elevator requests, and provides real-time status updates. The implementation follows N-Tier architecture principles and uses modern .NET technologies.

## API Endpoints

### 1. Get Elevator Status
- **Endpoint**: `GET /api/Elevator/status`
- **Description**: Retrieves the current status of all elevators
- **Response**: List of elevator statuses including:
  - Current floor
  - Current direction
  - State (Idle, Moving, DoorOpen, DoorClosing)
  - Number of pending requests

### 2. Request Elevator
- **Endpoint**: `POST /api/Elevator/request`
- **Description**: Submits a request for an elevator
- **Request Body**:
  ```json
  {
    "sourceFloor": 1,
    "destinationFloor": 5,
    "direction": "Up"
  }
  ```
- **Response**: 
  - 200 OK: Request submitted successfully
  - 400 Bad Request: Invalid request parameters

## How It Works

### Request Processing Flow
1. **Request Validation**
   - Validates floor numbers (1-10)
   - Ensures source and destination floors are different
   - Verifies direction matches floor movement

2. **Elevator Assignment**
   - First tries to find an idle elevator
   - Then looks for elevators going in the same direction
   - Finally selects the closest elevator

3. **Movement Control**
   - Elevator moves to source floor
   - Opens doors for passenger pickup
   - Moves to destination floor
   - Opens doors for passenger dropoff
   - Returns to idle state if no more requests

### Timing Constants
- Floor Travel Time: 10 seconds per floor
- Door Operation Time: 10 seconds
- Random Request Interval: 15 seconds

## Technology Stack

### Core Packages
1. **Microsoft.AspNetCore.App**
   - ASP.NET Core framework
   - Used for building the web API

2. **Swashbuckle.AspNetCore (6.6.2)**
   - Swagger/OpenAPI integration
   - Provides API documentation
   - Interactive API testing interface

3. **Microsoft.Extensions.Logging**
   - Logging framework
   - Used for tracking elevator operations

### Development Tools
1. **Microsoft.VisualStudio.Azure.Containers.Tools.Targets**
   - Docker container support
   - Enables containerized deployment

## Architecture

### N-Tier Structure
1. **API Layer** (`test-elevator.api`)
   - Controllers
   - API endpoints
   - Request/Response handling

2. **Core Layer** (`test-elevator.core`)
   - Business logic
   - Service implementations
   - Domain models

3. **Test Layer** (`test-elevator.tests`)
   - Unit tests
   - Integration tests
   - Test utilities

### Design Principles
- **SOLID Principles**
  - Single Responsibility
  - Open/Closed
  - Liskov Substitution
  - Interface Segregation
  - Dependency Inversion

- **DRY (Don't Repeat Yourself)**
  - Reusable components
  - Shared validation logic

- **KISS (Keep It Simple, Stupid)**
  - Clear, straightforward implementations
  - Minimal complexity

## Getting Started

1. **Prerequisites**
   - .NET 8.0 SDK
   - Visual Studio 2022 or later

2. **Running the Application**
   ```bash
   dotnet run
   ```
   - API will be available at: https://localhost:7094
   - Swagger UI: https://localhost:7094/swagger

3. **Running Tests**
   ```bash
   dotnet test
   ```

## Configuration
The application uses standard ASP.NET Core configuration in `appsettings.json`:
- Logging levels
- Environment settings
- Host configuration 