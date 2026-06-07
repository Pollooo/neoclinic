# NeoClinic Project Instructions

This project is a clinic management system with a .NET backend and an Angular frontend.

## Architecture

The backend follows Clean Architecture principles:

- **NeoClinic.Domain**: Contains domain entities and enums. It should have no dependencies on other projects or external libraries.
- **NeoClinic.Application**: Contains all business logic, MediatR commands/queries, and handlers. Defines interfaces for infrastructure services.
- **NeoClinic.Infrostructure**: Implements infrastructure interfaces, including EF Core `ApplicationDbContext` and external service integrations (Firebase, Telegram Bot).
- **NeoClinic.Api**: The entry point, using ASP.NET Core Minimal APIs.

## Backend Patterns & Conventions

- **CQRS with MediatR**: Every use case should be implemented as a MediatR Request (Command or Query) and a corresponding Handler within `NeoClinic.Application/UserCases`.
- **Validation**: Use FluentValidation. Validators are automatically executed via a `ValidationBehavior` in the MediatR pipeline.
- **Minimal APIs**: Endpoints are defined as static methods in `NeoClinic.Api/Endpoints` and mapped in `NeoClinic.Api/Endpoints/EndpointGroupBase.cs`.
- **Dependency Injection**: Each project has a `DependencyInjection.cs` file in its root to register its services.
- **Migrations**: EF Core migrations are located in `NeoClinic.Infrostructure/Migrations`. They are applied automatically on startup in development and production.

## Frontend Patterns & Conventions

- **Framework**: Angular (v21+) with SSR (Server-Side Rendering) and Hydration.
- **Structure**:
    - `core/`: Singleton services, guards, and interceptors.
    - `shared/`: Reusable components, directives, and pipes.
    - `features/`: Feature-specific modules/components.
- **Testing**: Vitest is used for unit testing.
- **Styling**: Vanilla CSS (prefer modern CSS features).

## Development Workflows

- **Adding a new Use Case**:
    1. Create entities in `NeoClinic.Domain` if necessary.
    2. Add Command/Query, Validator, and Handler in `NeoClinic.Application/UserCases/[Feature]`.
    3. Add Endpoint in `NeoClinic.Api/Endpoints/[Feature]Endpoints.cs`.
    4. Register the new Endpoint mapping in `EndpointGroupBase.cs`.
- **Database Changes**: Use `dotnet ef migrations add [Name] --project NeoClinic.Infrostructure --startup-project NeoClinic.Api`.

## External Integrations

- **Telegram Bot**: Managed via `TelegramBotReceiver` and handlers in `NeoClinic.Application`.
- **Storage**: Uses Firebase Storage (via `FirebaseStorageService`).
- **Auth**: JWT-based authentication with an "AdminPolicy" for protected endpoints.
