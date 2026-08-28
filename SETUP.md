# Local setup

This guide describes how to run B2B Commerce Demo locally on Windows.

## 1. Prerequisites

Install:

- .NET 10 SDK
- Docker Desktop
- Node.js and npm
- Git
- Visual Studio, Visual Studio Code or another compatible IDE

For Entity Framework Core commands, install the CLI tool if it is not already available:

```powershell
dotnet tool install --global dotnet-ef
```

## 2. Clone the repository

```powershell
git clone <REPOSITORY_URL>

cd B2BCommerceDemo
```

## 3. Start PostgreSQL

The development environment uses PostgreSQL 17 running in Docker.

From the repository root, start the database:

```powershell
docker compose up -d
```

Verify that the container is running:

```powershell
docker compose ps
```

The PostgreSQL configuration used by the project is defined in `docker-compose.yml` and the development configuration files.

## 4. Restore the backend

From the repository root:

```powershell
dotnet restore
```

## 5. Configure local secrets

Run the following commands from the API project directory:

```powershell
cd .\B2BCommerceDemo.API
```

The project uses .NET User Secrets for sensitive local configuration.

### Required application secrets

Configure the JWT signing key:

```powershell
dotnet user-secrets set "Jwt:Key" "<JWT_SIGNING_KEY>"
```

Configure the bootstrap administrator:

```powershell
dotnet user-secrets set "AdminBootstrap:Email" "<ADMIN_EMAIL>"
dotnet user-secrets set "AdminBootstrap:Password" "<STRONG_ADMIN_PASSWORD>"
```

### SMTP email

The application uses SMTP for account verification and order-related emails.

Configure the SMTP settings required by your email provider:

```powershell
dotnet user-secrets set "Email:SmtpHost" "<SMTP_HOST>"
dotnet user-secrets set "Email:SmtpPort" "<SMTP_PORT>"
dotnet user-secrets set "Email:Username" "<SMTP_USERNAME>"
dotnet user-secrets set "Email:Password" "<SMTP_PASSWORD>"
dotnet user-secrets set "Email:FromEmail" "<FROM_EMAIL>"
dotnet user-secrets set "Email:FromName" "<FROM_NAME>"
```

For providers such as Gmail, an app password may be required instead of the normal account password.

### Optional Icecat integration

Icecat product enrichment is optional and disabled by default.

If Icecat is enabled locally, configure the username associated with an Icecat account:

```powershell
dotnet user-secrets set "Icecat:Username" "<ICECAT_USERNAME>"
```

The portfolio version has been tested with Icecat enrichment using an Icecat account and username.

### Optional Rackbeat integration

Rackbeat ERP integration is optional and disabled by default in the portfolio configuration.

If you have access to a Rackbeat environment and explicitly enable the integration, configure its API key:

```powershell
dotnet user-secrets set "Rackbeat:ApiKey" "<RACKBEAT_API_KEY>"
```

Rackbeat credentials are not required to run the portfolio demo with the integration disabled.

### Verify User Secrets

To verify that the configured keys exist locally:

```powershell
dotnet user-secrets list
```

This command displays secret values, so its output must be treated as sensitive.

Do not add secret values to `appsettings.json`, screenshots, tickets or Git commits.

## 6. Apply database migrations

From the repository root, apply the Entity Framework Core migrations:

```powershell
dotnet ef database update --project .\B2BCommerceDemo.Infrastructure --startup-project .\B2BCommerceDemo.API
```

The application creates the bootstrap administrator from the configured `AdminBootstrap` values.

## 7. Run the backend

From the API project directory:

```powershell
cd .\B2BCommerceDemo.API
dotnet run
```

Use the HTTPS address shown in the console.

Swagger is available in the development environment through the API's configured launch URL.

## 8. Run the frontend

Open another PowerShell window and navigate to the frontend project:

```powershell
cd .\B2BCommerceDemo.Web

npm install
npm run dev
```

Open the Vite URL shown in the console.

If the API port changes, update the frontend development API base URL in the project's existing frontend configuration.

## 9. Verify the solution

Run the backend tests from the repository root:

```powershell
dotnet test
```

Run the frontend checks:

```powershell
cd .\B2BCommerceDemo.Web

npm run lint
npm run build
```

Recommended manual smoke test:

1. Start PostgreSQL, the API and the frontend.
2. Register a test business customer.
3. Confirm the registration email.
4. Log in as the bootstrap administrator.
5. Approve the pending company and assign the appropriate pricing configuration.
6. Log in as the approved customer.
7. Open the product catalogue and verify product/pricing information.
8. Add a product to the cart and complete a test checkout.
9. Confirm that the order appears in the customer order history and admin area.
10. Change the order status in the admin area and verify the expected email notification.
11. If Icecat has been configured and enabled, test product enrichment from the product administration area.
12. Leave Rackbeat disabled unless a valid test environment has intentionally been configured.

## 10. Optional integrations

The portfolio configuration is designed to run without Rackbeat or Icecat.

To use an optional integration:

1. Add the required local configuration using User Secrets.
2. Enable the integration in the development configuration.
3. Restart the API.

Do not enable an external integration unless you have permission to access the corresponding external service.

## 11. Production configuration

Do not use User Secrets in production.

Configure sensitive values through the selected hosting provider, environment variables or a managed secret store.

Environment-variable names follow ASP.NET Core's double-underscore convention, for example:

```text
Jwt__Key

AdminBootstrap__Email
AdminBootstrap__Password

Email__SmtpHost
Email__SmtpPort
Email__Username
Email__Password
Email__FromEmail
Email__FromName

Icecat__Username
Rackbeat__ApiKey

ConnectionStrings__DefaultConnection
```

Production database credentials, SMTP credentials and integration credentials should never be committed to the repository.