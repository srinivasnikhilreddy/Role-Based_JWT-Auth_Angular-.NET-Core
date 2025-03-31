# Role-Based JWT Authentication (Angular + .NET Core)

## 📌 Project Overview
This project implements JWT-based authentication and role-based authorization using ASP.NET Core Web API for the backend and Angular for the frontend. It allows users to log in, manage authentication tokens, and access protected routes based on roles (e.g., Admin, User).


## 🔐 Authentication & Authorization

* Users receive a JWT Token upon login.

* Angular stores the token in localStorage and automatically sends it with API requests using an HTTP Interceptor.

* The ASP.NET Core middleware validates tokens and checks roles before granting access.

* When the JWT token expires, the backend automatically generates a new token using the refresh token.

* If the refresh token also expires, the backend generates a new refresh token to ensure seamless authentication.


## 🏗 Tech Stack

**Frontend**: Angular 18 (Standalone APIs, Reactive Forms, HTTP Interceptors)

**Backend**: ASP.NET Core Web API (.NET 6/7)

**Database**: MSSQL (Entity Framework Core - Code First)

**Authentication**: JWT (JSON Web Token)

**Authorization**: Role-Based Access Control (RBAC)

**Styling & UI**: Bootstrap, SweetAlert2


## ✨ Features

✅ User authentication using JWT Tokens

✅ Role-based authorization (Admin, User, etc.)

✅ Secure API endpoints with JWT Middleware

✅ HTTP Interceptors for automatic token handling in Angular

✅ Refresh token mechanism for maintaining sessions

✅ Database with EF Core (Code-First)

✅ Login and Registration UI with SweetAlert2 Notifications


## 🎯 Future Enhancements for Similar Projects

* Single Sign-On (SSO) Integration (Google, GitHub, Facebook)

* OAuth2 Implementation for secure third-party authentication

* Multi-Factor Authentication (MFA) for enhanced security

* Logging & Monitoring using tools like Serilog & ELK Stack

* Microservices Architecture for scalability





