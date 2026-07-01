# PaymentGateway

A .NET 8 payment gateway built as a microservices solution. It provides authentication, product catalog, customer management, payment links, PIX payments, and checkout orchestration behind an API gateway.

## Architecture

The solution follows **Clean Architecture** per microservice:

```
Service/
├── Service.API            # Controllers, HTTP pipeline, DI composition
├── Service.Application    # Use cases, DTOs, validators, service interfaces
├── Service.Domain         # Entities, enums, repository interfaces
├── Service.Infrastructure # Dapper repositories, SQL migrations, HTTP clients
└── Service.Test           # Unit tests (controllers and services)
```

Shared libraries:

- **Shared.Kernel** — common result types and utilities
- **Shared.Infrastructure** — Dapper context, DbUp migrations, internal auth attributes

**Gateway.API** uses [YARP](https://microsoft.github.io/reverse-proxy/) as a reverse proxy with JWT authentication. Authenticated requests automatically receive an `X-User-Id` header forwarded to downstream services.

## Services

| Service       | Port | Responsibility                                      |
|---------------|------|-----------------------------------------------------|
| Gateway.API   | 5001 | API gateway, JWT validation, request routing        |
| Auth.API      | 5002 | User registration, login, JWT token issuance        |
| Customer.API  | 5003 | Customer CRUD and internal get-or-create            |
| Catalog.API   | 5004 | Products and prices                                 |
| PaymentLink.API | 5006 | Payment link creation and internal lookup         |
| Checkout.API  | 5007 | Checkout flow orchestration (PIX)                   |
| Payment.API   | 5008 | PIX payment creation and sandbox confirmation       |

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- YARP Reverse Proxy
- JWT Bearer authentication
- FluentValidation
- Dapper + SQL Server
- DbUp (SQL migrations on startup)
- xUnit + Moq (unit tests)

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local instance on port `1433`)
- JetBrains Rider, Visual Studio, or VS Code

Each service expects its own database. Default development connection strings use:

- **Server:** `localhost,1433`
- **User:** `sa`
- **Password:** `PaymentGateway@123`

Databases are created automatically by DbUp when each service starts.

## Getting Started

### 1. Clone and restore

```bash
git clone <repository-url>
cd PaymentGateway
dotnet restore PaymentGateway.sln
```

### 2. Start SQL Server

Ensure SQL Server is running and reachable at `localhost,1433` with the credentials configured in each service's `appsettings.Development.json`.

### 3. Run the services

Start each microservice (migrations run automatically on startup):

```bash
dotnet run --project src/Services/Auth/Auth.API
dotnet run --project src/Services/Customer/Customer.API
dotnet run --project src/Services/Catalog/Catalog.API
dotnet run --project src/Services/PaymentLink/PaymentLink.API
dotnet run --project src/Services/Checkout/Checkout.API
dotnet run --project src/Services/Payment/Payment.API
dotnet run --project src/Gateway/Gateway.API
```

In Rider, you can configure a **Compound Run Configuration** to start all services at once.

All public traffic should go through the gateway at `http://localhost:5001`.

### 4. Authenticate

Register or log in via the Auth service (through the gateway):

```http
POST http://localhost:5001/api/v1/auth/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "your-password"
}
```

Use the returned `accessToken` as a Bearer token for protected routes:

```http
Authorization: Bearer <accessToken>
```

## API Overview

| Route prefix                         | Auth required | Description                    |
|--------------------------------------|---------------|--------------------------------|
| `/api/v1/auth/*`                     | No            | Register and login             |
| `/api/v1/customers/*`                | Yes           | Customer management            |
| `/api/v1/products/*`                 | Yes           | Product catalog                |
| `/api/v1/paymentlinks/*`             | Yes           | Payment link creation          |
| `/api/v1/checkouts/*`                | No            | Checkout and PIX payment flow  |
| `/api/v1/customers/internal/*`       | Internal key  | Service-to-service customer ops |
| `/api/v1/prices/internal/*`          | Internal key  | Bulk price lookup              |
| `/api/v1/paymentlinks/internal/*`    | Internal key  | Payment link lookup            |
| `/api/v1/payments/internal/*`        | Internal key  | PIX payment operations         |

Internal endpoints require the `X-Internal-Api-Key` header. The default development key is `123321` (configured in `InternalSettings:ApiKey`).

## Testing

Unit tests cover controllers and application services. Repositories are not tested (reserved for future integration tests).

```bash
dotnet test PaymentGateway.sln
```

| Test project       | Tests |
|--------------------|-------|
| Auth.Test          | 13    |
| Catalog.Test       | 12    |
| Customer.Test      | 11    |
| Payment.Test       | 9     |
| Checkout.Test      | 9     |
| PaymentLink.Test   | 7     |

Tests use **xUnit** and **Moq**, referencing each service's `.API` project.

## Solution Structure

```
PaymentGateway/
├── PaymentGateway.sln
├── README.md
└── src/
    ├── Gateway/
    │   └── Gateway.API
    ├── Shared/
    │   ├── Shared.Kernel
    │   └── Shared.Infrastructure
    └── Services/
        ├── Auth/
        ├── Catalog/
        ├── Checkout/
        ├── Customer/
        ├── Payment/
        └── PaymentLink/
```

## Payment Flow (PIX)

1. Merchant creates **products** and **prices** in Catalog.
2. Merchant creates a **payment link** in PaymentLink (items reference catalog prices).
3. Customer opens the checkout page and calls Checkout to create a PIX payment.
4. Checkout orchestrates calls to PaymentLink, Customer, Catalog (prices), and Payment.
5. In sandbox mode, the payment can be confirmed via the checkout confirmation endpoint.
