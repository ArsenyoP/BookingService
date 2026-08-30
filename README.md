# BookingService

A comprehensive booking system built with .NET 9 following Clean Architecture principles.

## Overview

BookingService is a RESTful API for managing bookings of accommodations, rooms, and amenities. The system supports user authentication, role-based access control, advanced search capabilities using AI/RAG, email notifications, and various other features essential for a modern booking platform.

## Architecture

The project follows Clean Architecture with the following layers:

- **API Layer**: Presentation layer with Minimal API endpoints
- **Application Layer**: Contains use cases, DTOs, and application interfaces (MediatR)
- **Domain Layer**: Core business logic, entities, value objects, and domain events
- **Infrastructure Layer**: Data access, external services, and framework implementations

## Features

- 🔐 **Authentication & Authorization**: JWT-based auth with role management (Admin, User, Guest)
- 🏠 **Property Management**: Create, read, update, delete listings and rooms
- 🛏️ **Booking System**: Room availability checking, booking creation, cancellation
- 🏷️ **Amenities Management**: Add/remove amenities to listings and rooms
- ⭐ **Review System**: Users can rate and review properties
- 📧 **Email Notifications**: Automated emails for booking confirmations, cancellations, etc.
- 🔍 **AI-Powered Search**: RAG (Retrieval-Augmented Generation) with Qdrant for semantic search
- 💬 **AI Chat**: Chatbot for helping users find suitable accommodations
- 📦 **Outbox Pattern**: Reliable message delivery for distributed systems
- ⚡ **Rate Limiting**: IP-based rate limiting to prevent abuse
- 💾 **Caching**: Multiple caching strategies for improved performance
- 📝 **Logging**: Structured logging with Serilog
- 🩺 **Health Checks**: Endpoint for monitoring service health
- 🐳 **Docker Support**: Containerized deployment
- 📋 **Validation**: FluentValidation for request validation
- ⚠️ **Global Exception Handling**: Centralized error handling middleware

## Technology Stack

- **.NET 9**: Latest LTS version
- **Entity Framework Core 9**: ORM for data access
- **MediatR**: CQRS and mediator pattern implementation
- **FluentValidation**: Validation library
- **Serilog**: Structured logging
- **Qdrant**: Vector database for AI-powered search
- **ASP.NET Core Identity**: Authentication and authorization
- **Docker**: Containerization
- **Swagger/OpenAPI**: API documentation
- **HealthChecks**: Service health monitoring

## Project Structure

```
BookingService/
├── src/
│   ├── BookingService.API/          # API layer (Minimal API endpoints)
│   ├── BookingService.Application/  # Application layer (Use cases, DTOs)
│   ├── BookingService.Domain/       # Domain layer (Entities, business logic)
│   └── BookingService.Infrastructure/ # Infrastructure layer (Data, external services)
└── tests/
    ├── BookingService.UnitTests/    # Unit tests
    └── BookingService.IntegrationTests/ # Integration tests
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started) (optional, for containerized deployment)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or compatible database
- [Qdrant](https://qdrant.tech/) (for AI search features)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/BookingService.git
   cd BookingService
   ```

2. Configure the database connection in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=your_server;Database=BookingService;User Id=your_username;Password=your_password;"
     }
   }
   ```

3. Configure Qdrant settings (if using AI search):
   ```json
   {
     "QdrantSettings": {
       "Url": "http://localhost:6333",
       "ApiKey": "your_qdrant_api_key"
     }
   }
   ```

4. Run database migrations:
   ```bash
   dotnet ef database update --project src/BookingService.Infrastructure --startup-project src/BookingService.API
   ```

5. Run the application:
   ```bash
   dotnet run --project src/BookingService.API
   ```

### Docker Deployment

```bash
docker build -t bookingservice .
docker run -p 5000:80 bookingservice
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user
- `POST /api/auth/refresh-token` - Refresh access token

### Listings
- `GET /api/listing` - Get all listings (with filtering/pagination)
- `GET /api/listing/{id}` - Get listing by ID
- `POST /api/listing` - Create new listing
- `DELETE /api/listing/{id}` - Delete listing

### Rooms
- `GET /api/room` - Get all rooms (with filtering/pagination)
- `GET /api/room/{id}` - Get room by ID
- `POST /api/room` - Create new room
- `DELETE /api/room/{id}` - Delete room

### Bookings
- `GET /api/booking` - Get all bookings (with filtering/pagination)
- `GET /api/booking/{id}` - Get booking by ID
- `POST /api/booking` - Create new booking
- `DELETE /api/booking/{id}` - Cancel booking

### Amenities
- `GET /api/amenity` - Get all amenities
- `POST /api/amenity` - Create new amenity
- `DELETE /api/amenity/{id}` - Delete amenity

### Reviews
- `GET /api/review` - Get all reviews (with filtering/pagination)
- `GET /api/review/{id}` - Get review by ID
- `POST /api/review` - Create new review

### AI Features
- `GET /api/chat` - Chat with AI assistant for room recommendations
- `GET /api/weather/{location}` - Get weather information for location

## Configuration

Key configuration sections in `appsettings.json`:

- `ConnectionStrings`: Database connection strings
- `QdrantSettings`: Configuration for AI vector search
- `EmailSettings`: SMTP configuration for email notifications
- `JwtSettings`: JWT token configuration
- `RateLimiting`: Rate limiting policies
- `CacheSettings`: Caching configuration
- `Serilog`: Logging configuration

## Testing

Run unit tests:
```bash
dotnet test tests/BookingService.UnitTests
```

Run integration tests:
```bash
dotnet test tests/BookingService.IntegrationTests
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with .NET 9 and ASP.NET Core
- Inspired by clean architecture principles
- Uses various open-source libraries as mentioned in the technology stack