# Local setup

This guide describes how to run B2B Commerce Demo locally on Windows.

## 1. Prerequisites

Install:

- .NET 10 SDK
- SQL Server LocalDB
- Node.js and npm
- Git
- Visual Studio 2022 or another compatible IDE

For Entity Framework Core commands, install the CLI tool if it is not already available:

```powershell
dotnet tool install --global dotnet-ef
```

## 2. Clone the repository

```powershell
git clone <REPOSITORY_URL>
cd B2BCommerceDemo
```

## 3. Restore the backend

```powershell
dotnet restore
```

## 4. Configure local secrets

Run the following commands from the API project directory:

```powershell
cd .\B2BCommerceDemo.API
```

The project uses .NET User Secrets for sensitive local configuration. Set the following keys with valid values:

```powershell
dotnet user-secrets set "Jwt:Key" "<JWT_SIGNING_KEY>"
dotnet user-secrets set "Rackbeat:ApiKey" "<RACKBEAT_API_KEY>"
dotnet user-secrets set "Icecat:Username" "<ICECAT_USERNAME>"
dotnet user-secrets set "AdminBootstrap:Email" "<ADMIN_EMAIL>"
dotnet user-secrets set "AdminBootstrap:Password" "<STRONG_ADMIN_PASSWORD>"
```

Do not add these values to `appsettings.json`, screenshots, tickets or Git commits.

To verify that the keys exist locally:

```powershell
dotnet user-secrets list
```

This command displays the values, so its output must be treated as sensitive.

## 5. Database

The default development connection string uses SQL Server LocalDB:

```text
Server=(localdb)\MSSQLLocalDB;Database=B2BCommerceDemoDb;Trusted_Connection=True;TrustServerCertificate=True
```

Apply the Entity Framework Core migrations from the repository root:

```powershell
dotnet ef database update --project .\B2BCommerceDemo.Infrastructure --startup-project .\B2BCommerceDemo.API
```

The application seeds the required initial data when configured to do so. The bootstrap administrator is created from the `AdminBootstrap` User Secrets values.

## 6. Run the backend

From the API project directory:

```powershell
dotnet run
```

Use the HTTPS address shown in the console. Swagger is available in the development environment through the API's configured launch URL.

## 7. Run the frontend

Open another PowerShell window:

```powershell
cd .\B2BCommerceDemo.Web
npm install
npm run dev
```

Open the Vite URL shown in the console.

If the API port changes, update the frontend development API base URL in the project's existing frontend configuration.

## 8. Verify the solution

Backend tests:

```powershell
dotnet test
```

Frontend checks:

```powershell
cd .\B2BCommerceDemo.Web
npm run lint
npm run build
```

Recommended manual smoke test:

1. Start the API and frontend.
2. Log in as the bootstrap administrator.
3. Open the product catalogue.
4. Add a product to the cart.
5. Complete a test checkout if the environment permits it.
6. Confirm that the order appears in the order history/admin area.
7. Run a safe Rackbeat read or product-sync operation.
8. Confirm that Icecat-enriched product content is displayed where available.

## 9. Production configuration

Do not use User Secrets in production. Configure secrets through the selected hosting provider, environment variables or a managed secret store.

Environment-variable names follow ASP.NET Core's double-underscore convention, for example:

```text
Jwt__Key
Rackbeat__ApiKey
Icecat__Username
AdminBootstrap__Email
AdminBootstrap__Password
ConnectionStrings__DefaultConnection
```

