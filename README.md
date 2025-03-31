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


### System Architecture 
![Image](https://github.com/user-attachments/assets/f4eae86a-fca4-49d9-8927-ac10bba0aeee)


### Project Flow
![Image](https://github.com/user-attachments/assets/cb271488-2217-4358-a93c-e893a5ac2772)


### DATABASE
### Users table
![Image](https://github.com/user-attachments/assets/8cb17e67-d330-4ed6-83e5-809609571d89)


### Refresh tokens table 
![Image](https://github.com/user-attachments/assets/f8d3ae0c-e2bd-449b-807b-22279841e7cf)



### Frontend Client (Angular18)
### login with Admin credentials:
![Image](https://github.com/user-attachments/assets/eccd05d3-04d3-4a76-95de-8ea3fb18e5fb)


### Got access to Admin data:
![Image](https://github.com/user-attachments/assets/50a0a000-3b77-4fa3-88e9-cfa8843d0639)


### Access denied for the user data
![Image](https://github.com/user-attachments/assets/00ad0852-95a3-4da2-9f19-b3fd67adb9aa)


### POSTMAN testing
### JWT Token generation with Admin credentials
![Image](https://github.com/user-attachments/assets/99303e2e-53d6-4a00-a4f2-e3be86e514f9)


## Got access to Admin data
![Image](https://github.com/user-attachments/assets/7ea03b79-beec-4397-bf10-dd42a69b948d)


### Access denied for the user data
![Image](https://github.com/user-attachments/assets/25760235-48b9-4ef5-9c43-2476b3799897)


