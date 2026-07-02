# Ticket Notification API

A Clean Architecture ASP.NET Core Web API for managing support tickets and orchestrating multi-channel notifications (**Email**, **SMS**, **Push**) with built-in retry logic and idempotent delivery.

---

## Features

* Create and manage support tickets
* Send notifications through multiple channels
* Automatic retry for failed deliveries
* Idempotent notification endpoints
* Extensible channel architecture using the Strategy Pattern
* Clean separation of concerns via Ports and Adapters

---

## Tech Stack

* ASP.NET Core (.NET 8)
* C#
* Swagger
* xUnit (Unit Testing)

---

## Getting Started

### Prerequisites

* .NET 8 SDK or later

Verify installation:

```bash
dotnet --version
```

### Run the API

```bash
dotnet run --project src/TicketNotificationApi.Api
```

After startup, open Swagger UI (check console output for the exact URL), typically:

```text
https://localhost:<port>/swagger
```

### Run Tests

```bash
dotnet test
```

---

## Project Structure

```text
src/
├── TicketNotificationApi.Domain
├── TicketNotificationApi.Application
├── TicketNotificationApi.Infrastructure
└── TicketNotificationApi.Api
```

---

## Architecture

This solution follows **Clean Architecture (Ports and Adapters)**.

Dependency flow always points inward:

```text
Domain ← Application ← Infrastructure
                  ↑
                 API
```

### Domain

Zero external dependencies.

Contains:

* Core entities
* Value objects
* Enums
* Business rules

This layer represents the heart of the system.

---

### Application

Depends **only on Domain**.

Contains:

* Use cases / orchestration logic
* DTOs
* Interfaces (Ports)
* Validation and business workflows

This layer defines how the system behaves.

---

### Infrastructure

Depends on Application.

Contains concrete implementations of application ports:

* Repositories
* Notification senders
* External service integrations
* Storage adapters

This layer handles external concerns.

---

### API

ASP.NET Core entry point.

Responsibilities:

* HTTP routing
* Request/response handling
* Model binding
* Status codes
* Dependency injection configuration

Controllers remain intentionally thin.

---

## Composition Root

`Program.cs` is the **only place allowed to reference Infrastructure directly**.

Its sole purpose is wiring dependencies:

* Register repositories
* Register notification senders
* Configure middleware

This keeps application logic independent from infrastructure concerns.

---

## Design Decisions

### Ports and Adapters

Application owns the interfaces.

Infrastructure provides implementations.

This makes business logic independent from databases, APIs, and messaging providers.

---

### Thin Controllers

Controllers should not contain business logic.

They only:

* Receive HTTP requests
* Delegate to application services
* Return HTTP responses

---

### Strategy Pattern for Notification Channels

Notification delivery uses:

```csharp
IEnumerable<INotificationSender>
```

Each sender handles one channel:

* Email
* SMS
* Push

Adding a new channel (for example Slack) requires:

1. Creating a new sender in Infrastructure
2. Registering it in dependency injection

No changes are required in Domain, Application, or API layers.

---

### Idempotent Notification Endpoint

Calling:

```text
/{id}/notify
```

multiple times is safe.

Already-sent notifications are skipped automatically, preventing duplicate delivery.

---

### Retry Policy

Failed notification attempts increment a retry counter.

Retries stop after **3 attempts** to prevent infinite loops and reduce resource waste.

---

## API Overview

Example endpoints:

| Method | Endpoint               | Description           |
| ------ | ---------------------- | --------------------- |
| GET    | `/tickets/{id}`        | Get ticket details    |
| POST   | `/tickets`             | Create a ticket       |
| POST   | `/tickets/{id}/notify` | Trigger notifications |

See Swagger UI for the full contract.

---

## Testing

Unit tests cover:

* Notification orchestration
* Retry behavior
* Idempotency logic
* Channel selection

Run all tests:

```bash
dotnet test
```

---
