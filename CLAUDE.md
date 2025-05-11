# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a demo backend application built with ASP.NET Core that showcases integration between:
- .NET 8.0 Web API
- Zitadel identity service for authentication
- Casbin for policy-based authorization
- Multi-tenant architecture with tenant-specific access control

The application demonstrates a role-based access control system where permissions are enforced based on:
- User roles from Zitadel
- Tenant information (extracted from user metadata in JWT tokens)
- API endpoint path
- HTTP method

## Build and Run Commands

```bash
# Build the project
dotnet build

# Run the application
dotnet run --project Service

# Clean the project
dotnet clean

# Publish the application
dotnet publish -c Release
```

The service runs on:
- http://localhost:5258

## Key Architecture Components

1. **Authentication**: JWT Bearer tokens from Zitadel (configured to connect to localhost:8080)

2. **Claims Transformation**:
   - `ZitadelRoleClaimsTransformer.cs` extracts roles and tenant information from Zitadel JWT tokens
   - Roles from "urn:zitadel:iam:org:project:318999638361243651:roles" claim
   - Tenant from "urn:zitadel:iam:user:metadata" claim (Base64 encoded)

3. **Authorization**:
   - `CasbinAuthorizationMiddleware.cs` enforces permissions using Casbin policies
   - `model.conf` defines the multi-tenant authorization model
   - `policy.csv` contains authorization rules in the format: role, tenant, path, method
   - `PolicyManager.cs` provides methods to manage policies programmatically

4. **API Endpoints**:
   - `/api/hello/{text}`: Simple endpoint requiring authentication

## Configuration

- The application is configured to work with:
  - Zitadel running on http://localhost:8080
  - Frontend Angular application on http://localhost:4200

- CORS is configured to allow requests from the Angular frontend

## Dependencies

- Casbin.NET - Policy enforcement library
- Microsoft.AspNetCore.Authentication.JwtBearer - JWT authentication
- Microsoft.AspNetCore.OpenApi - API documentation
- Swashbuckle.AspNetCore - Swagger UI integration