# Security Guidelines

## Overview

This document provides important security guidelines for setting up and deploying the Dragon Ball Character Library application.

## Sensitive Data Management

### Configuration Files

The repository contains template configuration files with placeholder values. **Never commit real credentials or secrets to the repository.**

### Files with Placeholders

The following files contain placeholders that must be replaced with your actual values:

1. **`DragonBallLibrary.AppHost/appsettings.Development.json`**
   - `{your-azure-subscription-id}`: Your Azure subscription ID
   - `{your-sql-server}`: Your Azure SQL server name
   - `{your-database}`: Your database name
   - `{your-username}`: Your SQL database username
   - `{your-password}`: Your SQL database password

2. **Dapr Component Files** (`dapr/components/*.yaml`)
   - `{your-azure-client-id}`: Your Azure Service Principal Client ID
   - `{your-azure-tenant-id}`: Your Azure Tenant ID
   - `{your-keyvault-name}`: Your Azure Key Vault name
   - `{your-appconfig-name}`: Your Azure App Configuration name
   - `{your-storage-account-name}`: Your Azure Storage account name

3. **Deployment Files** (`deploy/azure-container-apps/*.yaml`)
   - `{your-sql-server}`: Your Azure SQL server name
   - `{your-database}`: Your database name
   - `{your-username}`: Your SQL database username
   - `{your-password}`: Your SQL database password
   - `{your-storage-account-key}`: Your Azure Storage account key

4. **Environment Files** (`DragonBallLibrary.Web/.env.*.example`)
   - Copy these files to `.env.development`, `.env.production`, and `.env.staging`
   - Update the API URLs with your actual endpoints
   - These `.env` files are ignored by git

## Best Practices

### 1. Environment Variables

- Use environment variables for sensitive configuration
- Never hardcode credentials in source code
- Use Azure Key Vault for production secrets

### 2. Local Development

- Copy `.env.*.example` files to `.env.*` for local development
- Keep your local `.env` files private (they are in `.gitignore`)
- Use `appsettings.Development.json` for local development only
- Never commit `appsettings.Development.json` with real credentials

### 3. Azure Key Vault

- Store all production secrets in Azure Key Vault
- Configure Dapr components to use Key Vault for secret management
- Use Managed Identity for authentication when possible

### 4. Connection Strings

- Never include passwords in connection strings in source control
- Use Azure Key Vault references in production
- For local development, use user secrets or environment variables

### 5. Azure Service Principal

- Create a dedicated Service Principal for the application
- Grant minimum required permissions (principle of least privilege)
- Rotate credentials regularly
- Never commit Service Principal credentials

## Setup Instructions

### For Local Development

1. Copy environment file templates:
   ```bash
   cd DragonBallLibrary.Web
   cp .env.development.example .env.development
   cp .env.production.example .env.production
   cp .env.staging.example .env.staging
   ```

2. Update `appsettings.Development.json` with your local Azure resources

3. Ensure you're authenticated with Azure CLI:
   ```bash
   az login
   ```

### For Production Deployment

1. Create an Azure Key Vault
2. Store all secrets in Key Vault
3. Configure Managed Identity for your Container Apps
4. Update Dapr components to reference Key Vault
5. Never use hardcoded credentials in production

## Reporting Security Issues

If you discover a security vulnerability, please email the repository maintainers directly. Do not create a public GitHub issue.

## Additional Resources

- [Azure Key Vault Documentation](https://docs.microsoft.com/azure/key-vault/)
- [Dapr Secrets Management](https://docs.dapr.io/operations/components/setup-secret-store/)
- [.NET User Secrets](https://docs.microsoft.com/aspnet/core/security/app-secrets)
- [Azure Managed Identity](https://docs.microsoft.com/azure/active-directory/managed-identities-azure-resources/)
