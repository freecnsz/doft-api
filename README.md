# DoFT Backend – Task Management API

**DoFT (Do Not Forget)** is a modern task management platform. This backend is built using **.NET 8.0** and follows **Clean Architecture** principles, offering a modular, maintainable, and scalable foundation for personal productivity systems.

---

## 🔧 Project Structure

This backend solution is organized into four main layers:

- **Domain Layer**  
  Contains core domain models and business rules.

- **Application Layer**  
  Contains business logic, services, and use-case definitions. Implements **CQRS** via **MediatR**.

- **Infrastructure Layer**  
  Deals with database access (PostgreSQL), file storage (AWS S3), and external dependencies.

- **Web API Layer**  
  Handles HTTP requests and routes. Serves as the entry point for the application.

---

## 🚀 Technologies Used

### Core Technologies
- [.NET 8.0](https://dotnet.microsoft.com/en-us/)
- ASP.NET Core Web API
- PostgreSQL + Entity Framework Core

### Security
- ASP.NET Core Identity
- JWT (JSON Web Tokens) Authentication
- Role-based Authorization
- Bearer Token Authentication
- Secure Password Hashing Policies

### API & Docs
- **Swagger / OpenAPI** via `Swashbuckle.AspNetCore`

### Logging & Monitoring
- **Serilog** with Console & File sinks
- Structured Logging
- Exception Middleware

### File Management
- **AWS S3** Integration
- Profile image upload and secure file storage

### Validation
- **FluentValidation** for request validation
- Global Exception Handling
- Custom Validation Middleware

---

## 🛠 Key Features

- ✅ **User Authentication & Authorization**
  - JWT + Refresh Token support
  - Secure password policies
  - Role-based access control

- 📁 **File Uploads**
  - AWS S3 integration
  - Profile picture and secure asset management

- 🧰 **Task Management Logic**
  - CQRS pattern with MediatR
  - Clean separation of concerns

- 🔒 **API Security**
  - HTTPS enforced
  - CORS configuration
  - Rate limiting
  - Secured Swagger endpoints

- 🏗 **Deployment-Ready**
  - Dockerized with multi-stage builds
  - Environment-based configuration
  - GitHub Actions support

---

## 🧱 Architectural Highlights

- Clean Architecture (by Uncle Bob principles)
- Domain-Driven Design (DDD)
- SOLID principles
- CQRS with MediatR
- Repository + Unit of Work Pattern
- Dependency Injection throughout
