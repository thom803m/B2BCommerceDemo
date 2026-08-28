# B2B Commerce Demo

B2B Commerce Demo is an anonymized portfolio version of a full-stack B2B e-commerce solution developed for business customers.

The application covers the complete flow from company registration and approval to product browsing, company-specific pricing, checkout, order management and email notifications. It also includes administration tools, CSV import/export and optional integrations with external ERP and product-content services.

## Tech stack

### Backend
- C# / .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- ASP.NET Core Identity
- PostgreSQL
- JWT authentication
- xUnit

### Frontend
- React
- TypeScript
- Vite
- Material UI
- Axios

### Infrastructure and tooling
- Docker
- Git / GitHub
- GitHub Actions
- Swagger / OpenAPI
- MailKit / SMTP

## Solution structure

- **B2BCommerceDemo.API** - ASP.NET Core Web API, authentication, authorization, middleware and HTTP endpoints.
- **B2BCommerceDemo.Core** - Domain models, DTOs, interfaces, events and shared business contracts.
- **B2BCommerceDemo.Infrastructure** - Entity Framework Core, ASP.NET Identity, application services, persistence, email handling and external integrations.
- **B2BCommerceDemo.Tests** - Unit and integration tests.
- **B2BCommerceDemo.Web** - React, TypeScript, Vite and Material UI frontend.

## Main functionality

### Customer and company management
- Business customer registration
- Email verification
- Administrator approval of new companies
- Company-specific price-group assignment
- Company suspension and account management
- JWT-based authentication and role-based authorization

### Product catalogue
- Product search and filtering
- Company-specific pricing
- Stock availability
- Incoming quantities and expected delivery dates
- Product descriptions, specifications and images
- CSV product import and export

### Shopping and orders
- Shopping cart
- Checkout
- Order history
- Administrative order management
- Order status workflow
- Email notifications when orders are received and confirmed

### Administration
- Company approval and management
- Product administration
- Order administration
- Pricing and price-group management
- Integration management

## Sample data

Sample CSV files are included in the repository under:

`sample-data/`

They can be used to test the application's CSV import functionality without having to create import files manually:

- [Download products.csv](sample-data/products.csv) - sample product catalogue data
- [Download delivery-dates.csv](sample-data/delivery-dates.csv) - sample incoming quantities and expected delivery dates

After logging in as an administrator, open the product administration area and select **Import data**. Use the **Products** or **Delivery dates** tab depending on the file being imported.

The sample data is intended for local development and portfolio demonstration purposes only.

## External integrations

The project contains integrations for:

- **Rackbeat ERP** - products, stock, customers and orders
- **Icecat** - product descriptions, specifications and images
- **SMTP email (MailKit)** - email verification, company registration/approval notifications and order-status emails

Rackbeat and Icecat are optional integrations and are disabled by default in the portfolio configuration. They require separate credentials and configuration to use.

Icecat product enrichment can be tested with the portfolio version and can be enabled locally with an Icecat account and username.

## Database

The application uses **PostgreSQL** through Entity Framework Core.

A local PostgreSQL 17 database can be started using Docker. Database migrations are handled through Entity Framework Core.

See [SETUP.md](SETUP.md) for the complete local setup instructions.

## Testing and CI/CD

The project uses GitHub Actions for continuous integration and deployment preparation.

On pushes to the repository, the CI workflow automatically runs:

- Backend restore and build
- Backend automated tests
- Frontend dependency installation
- Frontend lint
- Frontend production build

For the `main` branch, the workflow also creates deployment-ready artifacts for the backend API and frontend application.

This provides automated validation of changes before deployment and demonstrates the build/package stage of a CI/CD pipeline. Actual production deployment is intentionally not configured for this portfolio repository.

From the repository root, the main checks can also be run locally:

```powershell
dotnet test

cd .\B2BCommerceDemo.Web
npm install
npm run lint
npm run build
```

## Security

Sensitive development configuration is stored using **.NET User Secrets** and is not committed to Git.

This includes credentials and secrets for authentication, email and optional external integrations.

Production secrets should be supplied through the hosting platform's secret or environment-variable management.

The required local configuration keys are documented in [SETUP.md](SETUP.md), but their values are intentionally not included in this repository.

## Portfolio version

This repository is an anonymized portfolio/demo version of software originally developed as part of a professional B2B e-commerce project.

Company-specific branding, confidential information, production credentials and other sensitive information have been removed.

The portfolio version is intended to demonstrate the architecture, development process and functionality of the application rather than provide access to the original production environment.
