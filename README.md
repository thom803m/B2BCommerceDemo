# B2B Commerce Demo

B2B Commerce Demo is an anonymized portfolio version of a B2B webshop developed for approved business customers. The solution presents product, price, stock and delivery information from an ERP integration and enriches product content through Icecat.

## Solution overview

- **B2BCommerceDemo.API** - ASP.NET Core Web API, authentication, authorization and HTTP endpoints.
- **B2BCommerceDemo.Core** - domain models, DTOs, interfaces, events and shared business contracts.
- **B2BCommerceDemo.Infrastructure** - Entity Framework Core, ASP.NET Identity, services, persistence and external integrations.
- **B2BCommerceDemo.Tests** - unit and integration tests.
- **B2BCommerceDemo.Web** - React, TypeScript, Vite and Material UI frontend.

## Main functionality

- Company registration, approval and price-group assignment
- JWT-based login and role-based access
- Product catalogue with company-specific prices
- Stock levels and expected delivery dates
- Shopping cart, checkout and order history
- Administration of companies, products and orders
- ERP integration for products, stock, customers and orders
- Icecat enrichment of descriptions, specifications and images
- CSV product import and export

## Documentation

- [Local setup](SETUP.md)

## Security

Sensitive development configuration must be stored in .NET User Secrets and must not be committed to Git. Production secrets should be supplied through the hosting platform's secret or environment-variable management.

The currently required local secret keys are documented in `SETUP.md`; their values are intentionally not included in this repository.

## Quick verification

From the repository root:

```powershell
dotnet test

cd .\B2BCommerceDemo.Web
npm install
npm run lint
npm run build
```

## Portfolio version

This repository is an anonymized portfolio/demo version of software originally developed as part of a professional B2B e-commerce project.

Company-specific branding, confidential information, production data and credentials are not included. External integrations require the developer's own configuration and credentials to run locally.
