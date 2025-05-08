# Elevator Control System API

## 1. Overview

This is a RESTful API for controlling a multi-elevator system. The system manages multiple elevators, handles elevator requests, and provides real-time status updates. The implementation follows N-Tier architecture principles and uses modern .NET technologies.

Key Features:

- Multi-elevator management
- Real-time elevator status monitoring
- Request-based elevator dispatching
- Simulation mode for testing
- Automatic request processing

## 2. API Endpoints

### Get Elevator Status

- **Endpoint**: `GET /api/Elevator/status`
- **Description**: Retrieves the current status of all elevators
- **Response**: List of elevator statuses including:
  - Current floor
  - Current direction
  - State (Idle, Moving, DoorOpen, DoorClosing)
  - Number of pending requests

### Request Elevator

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

### Start Simulation

- **Endpoint**: `POST /api/Elevator/start-simulation`
- **Description**: Starts the elevator simulation process
- **Response**: 200 OK when simulation starts successfully

### Stop Simulation

- **Endpoint**: `POST /api/Elevator/stop-simulation`
- **Description**: Stops the elevator simulation process
- **Response**: 200 OK when simulation stops successfully

## 3. Running the Application

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or later (optional)
- Docker (optional, for containerized deployment)

### Running Locally

1. **Using Visual Studio**

   ```bash
   # Open the solution in Visual Studio
   # Set test-elevator.api as the startup project
   # Press F5 or click "Start" to run
   ```

2. **Using .NET CLI**

   ```bash
   # Navigate to the API project directory
   cd test-elevator.api

   # Restore dependencies
   dotnet restore

   # Run the application
   dotnet run
   ```

3. **Using Docker**

   ```bash
   # Build the Docker image
   docker build -t elevator-api .

   # Run the container
   docker run -p 8080:8080 -p 8081:8081 elevator-api
   ```

### Accessing the API

- API Base URL: https://localhost:7094
- Swagger UI: https://localhost:7094/swagger
- HTTP endpoint: http://localhost:5095

## 4. Request Processing Flow

1. **Request Validation**

   - Validates floor numbers (1-10)
   - Ensures source and destination floors are different
   - Verifies direction matches floor movement

2. **Elevator Assignment**

   - First tries to find an idle elevator
   - Then looks for elevators going in the same direction
   - Finally selects the closest elevator

3. **Movement Control**

   - Elevator moves to source floor (10 seconds per floor)
   - Opens doors for passenger pickup (10 seconds)
   - Moves to destination floor (10 seconds per floor)
   - Opens doors for passenger dropoff (10 seconds)
   - Returns to idle state if no more requests

4. **Simulation Process**
   - Must be started explicitly via API
   - Processes all queued requests
   - Generates random requests every 15 seconds
   - Updates elevator states in real-time

## 5. Technology Stack

### Core Packages

1. **Microsoft.AspNetCore.App**

   - ASP.NET Core framework
   - Used for building the web API
   - Provides HTTP request handling

2. **Swashbuckle.AspNetCore (6.6.2)**

   - Swagger/OpenAPI integration
   - Provides API documentation
   - Interactive API testing interface
   - Auto-generates API documentation

3. **Microsoft.Extensions.Logging**
   - Logging framework
   - Used for tracking elevator operations
   - Provides structured logging

### Development Tools

1. **Microsoft.VisualStudio.Azure.Containers.Tools.Targets**
   - Docker container support
   - Enables containerized deployment
   - Simplifies container management

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
