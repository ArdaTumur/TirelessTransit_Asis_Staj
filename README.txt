# MyAPI – Bus Line Management API

## Overview

MyAPI is a RESTful ASP.NET Core Web API for managing bus line information. The project provides secure user authentication using JSON Web Tokens (JWT), role-based authorization for administrators and regular users, and a real-time chat feature powered by SignalR.

The application also includes a simple frontend served from the `wwwroot` folder for login, registration, bus line management, and chat.

## Features

 JWT-based user authentication.
 User registration and login.
 Role-based authorization (`Admin` and `User`).
 CRUD operations for bus lines.
 Soft delete functionality for bus lines.
 SQL Server database integration with Entity Framework Core.
 Real-time chat using SignalR with one conversation per user.
 Persistent chat history in SQL Server.
 Dedicated admin page for bus-line management.
 Static frontend pages for authentication and chat.

## Technologies Used

 Technology              Purpose                         
 ----------------------  ------------------------------- 
 ASP.NET Core (.NET 10)  Backend Web API                 
 Entity Framework Core   Database ORM                    
 SQL Server              Database                        
 JWT Authentication      Secure authentication           
 SignalR                 Real-time messaging             
 OpenAPI  Swagger       API documentation (Development) 

## Project Structure

```text
MyAPI
│
├── Controllers         # API endpoints
│   ├── AuthController.cs
│   └── BusLinesController.cs
│
├── Services            # Business logic
│   ├── AuthService.cs
│   └── BusLineService.cs
│
├── Models              # Database models
│   ├── User.cs
│   ├── BusLine.cs
│   └── ChatMessage.cs
│
├── DTOs                # Requestresponse models
│
├── Data                # Entity Framework DbContext
│
├── Hubs                # SignalR hub
│   └── ChatHub.cs
│
├── Migrations          # EF Core migrations
│
├── wwwroot             # Static frontend files
│   ├── login
│   ├── register
│   ├── hub
│   ├── app.js
│   └── index.html
│
├── Program.cs           # Application configuration
└── appsettings.json     # Configuration settings
```

## Authentication

The API uses JWT Bearer authentication. After a successful login, the server returns a JWT token that must be included in authenticated requests.

Authorization Header

```http
Authorization Bearer your_jwt_token
```

### User Roles

 Admin

   Create bus lines.
   Update bus lines.
   Delete bus lines.
   View bus lines.

 User

   View bus lines.
   Access the chat.

## API Endpoints

### Authentication

 Method  Endpoint         Description                              
 ------  ---------------  ---------------------------------------- 
 POST    `apiregister`  Register a new user.                     
 POST    `apilogin`     Authenticate user and receive JWT token. 

### Bus Lines

 Method  Endpoint               Access      
 ------  ---------------------  ----------- 
 GET     `apibus-lines`       User, Admin 
 GET     `apibus-lines{id}`  User, Admin 
 POST    `apibus-lines`       Admin       
 PUT     `apibus-lines{id}`  Admin       
 DELETE  `apibus-lines{id}`  Admin       

## Bus Line Data

Each bus line contains

 Driver name
 Bus plate number
 Origin city
 Destination city
 Route status (Scheduled, En Route, Delayed, Cancelled)
 Operating hours
 Created user
 Updated user
 Created date
 Updated date
 Soft delete flag

Deleted bus lines are not permanently removed from the database. Instead, they are marked as deleted and excluded from API responses.

## Real-Time Chat

The project includes a SignalR hub located at

```text
chatHub
```

Authenticated users can connect using their JWT token and exchange messages in real time through the provided frontend chat page.

## Getting Started

### Prerequisites

 .NET 10 SDK
 SQL Server
 Visual Studio 2022 or Visual Studio Code

### Installation

1. Clone the repository.

```bash
git clone repository-url
```

2. Navigate into the project.

```bash
cd MyAPI
```

3. Update the SQL Server connection string in `appsettings.json`.

4. Apply Entity Framework migrations.

```bash
dotnet ef database update
```

5. Run the application.

```bash
dotnet run
```

The API will start locally and serve both the REST API and the static frontend.

## Frontend Pages

The application serves static pages from `wwwroot`.

 Page              Purpose                 
 ----------------  ----------------------- 
 `login`          User login page.        
 `register`       User registration page. 
 ``               Main application page.  
 `hub/chat.html` SignalR chat interface.
 `admin/bus-lines` Admin-only bus-line management page. 

## Security

 Passwords are hashed before being stored in the database.
 JWT tokens are validated for issuer, audience, lifetime, and signature.
 Authorization is enforced through ASP.NET Core role-based policies.

## Future Improvements

 Refresh token support.
 Password salting per user.
 Bus line search and filtering.
 Pagination for API responses.
 Chat message persistence in the database.
 Validation and error handling improvements.

## Author

Developed as an ASP.NET Core Web API project demonstrating authentication, authorization, Entity Framework Core, SQL Server integration, and SignalR real-time communication.
