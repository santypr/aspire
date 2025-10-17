# 🐉 Dragon Ball Character Library

A comprehensive full-stack application built with .NET Aspire, React.js, and Azure Container Apps, demonstrating modern cloud-native architecture with Dapr orchestration.

![Dragon Ball Character Library](https://github.com/user-attachments/assets/56a20ee0-6350-4a43-893c-7482c059d3a2)

## 🚀 Overview

This project showcases how to build a modern, cloud-native application using:

- **.NET Aspire** for orchestration and service discovery
- **React.js + TypeScript** for the frontend
- **ASP.NET Core Web API** for the backend
- **Azure Container Apps** for deployment
- **Dapr** for microservices integration
- **Azure Services** (App Configuration, Key Vault, Blob Storage, SQL Database)

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   React.js      │    │  .NET Web API   │    │  Azure Services │
│   Frontend      │◄──►│   Backend       │◄──►│                 │
│                 │    │                 │    │ • App Config    │
│ • TypeScript    │    │ • Entity FW     │    │ • Key Vault     │
│ • Modern UI     │    │ • Swagger       │    │ • Blob Storage  │
│ • CRUD Ops      │    │ • Health Checks │    │ • SQL Database  │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                        │                        │
         └────────────────────────┼────────────────────────┘
                                  │
                    ┌─────────────────┐
                    │ .NET Aspire     │
                    │ Orchestration   │
                    │                 │
                    │ • Service Disc. │
                    │ • Configuration │
                    │ • Observability │
                    └─────────────────┘
                                  │
                    ┌─────────────────┐
                    │ Azure Container │
                    │ Apps + Dapr     │
                    │                 │
                    │ • Auto Scaling  │
                    │ • Load Balancer │
                    │ • Ingress       │
                    └─────────────────┘
```

## 🎯 Features

### Dragon Ball Character Management
- ✅ **Create** new Dragon Ball characters
- ✅ **Read** character details and library
- ✅ **Update** character information
- ✅ **Delete** characters from library

### Character Attributes
- **Name**: Character's name (e.g., Goku, Vegeta)
- **Race**: Character's species (e.g., Saiyan, Namekian)
- **Planet**: Home planet (e.g., Earth, Vegeta)
- **Transformation**: Ultimate form (e.g., Ultra Instinct, Super Saiyan Blue)
- **Technique**: Signature attack (e.g., Kamehameha, Final Flash)

### Technical Features
- 🎨 **Modern UI**: Responsive design with Dragon Ball theme
- 🔒 **Type Safety**: Full TypeScript support
- 🏥 **Health Checks**: Comprehensive monitoring
- 📊 **API Documentation**: Swagger/OpenAPI integration
- 🔄 **Real-time Updates**: Live data synchronization
- 🛡️ **Error Handling**: Robust error management
- 📱 **Mobile Responsive**: Works on all device sizes

## 🛠️ Technology Stack

### Frontend
- **React.js 18** - UI Library
- **TypeScript** - Type Safety
- **CSS3** - Modern Styling
- **Axios** - HTTP Client
- **Create React App** - Build Tool

### Backend
- **.NET 8.0** - Runtime
- **ASP.NET Core** - Web Framework
- **Entity Framework Core** - ORM
- **Swagger** - API Documentation
- **.NET Aspire** - Orchestration

### Azure Services
- **Azure Container Apps** - Hosting
- **Azure App Configuration** - Configuration Management
- **Azure Key Vault** - Secrets Management
- **Azure Blob Storage** - File Storage
- **Azure SQL Database** - Data Storage

### DevOps & Orchestration
- **Dapr** - Microservices Runtime
- **Docker** - Containerization
- **YAML** - Configuration as Code
- **.NET Aspire** - Local Development

## 🚦 Getting Started

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+ and npm
- Docker Desktop
- Visual Studio 2022 or VS Code
- Azure CLI (for Azure deployments)
- Azure subscription (for cloud deployment)

### Local Development

> **⚠️ Important:** Before running the application, you must configure your credentials. See [SECURITY.md](SECURITY.md) for detailed instructions.

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd aspire
   ```

2. **Configure environment files**
   ```bash
   cd DragonBallLibrary.Web
   cp .env.development.example .env.development
   cp .env.production.example .env.production
   cp .env.staging.example .env.staging
   cd ..
   ```
   
   Then update `DragonBallLibrary.AppHost/appsettings.Development.json` with your Azure credentials (see [SECURITY.md](SECURITY.md)).

3. **Restore .NET packages**
   ```bash
   dotnet restore
   ```

4. **Install React dependencies**
   ```bash
   cd DragonBallLibrary.Web
   npm install
   cd ..
   ```

5. **Run with Aspire**
   ```bash
   cd DragonBallLibrary.AppHost
   dotnet run
   ```

6. **Access the applications**
   - **Aspire Dashboard**: http://localhost:15888
   - **API**: http://localhost:5304
   - **Frontend**: http://localhost:3000

### Manual Development (Alternative)

**Terminal 1 - API Service:**
```bash
cd DragonBallLibrary.ApiService
dotnet run
```

**Terminal 2 - React Frontend:**
```bash
cd DragonBallLibrary.Web
npm start
```

## 🌐 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/characters` | Get all characters |
| `GET` | `/api/characters/{id}` | Get character by ID |
| `POST` | `/api/characters` | Create new character |
| `PUT` | `/api/characters/{id}` | Update character |
| `DELETE` | `/api/characters/{id}` | Delete character |
| `GET` | `/health` | Health check |
| `GET` | `/swagger` | API documentation |

### Example Character JSON:
```json
{
  "id": 1,
  "name": "Goku",
  "race": "Saiyan",
  "planet": "Earth",
  "transformation": "Ultra Instinct",
  "technique": "Kamehameha"
}
```

## 📦 Azure Container Apps Deployment

### 1. Build Container Images
```bash
# API Service
docker build -f DragonBallLibrary.ApiService/Dockerfile -t dragonball-api .

# React Frontend
docker build -f DragonBallLibrary.Web/Dockerfile -t dragonball-web .
```

### 2. Push to Azure Container Registry
```bash
# Tag images
docker tag dragonball-api dragonballregistry.azurecr.io/dragonball-api:latest
docker tag dragonball-web dragonballregistry.azurecr.io/dragonball-web:latest

# Push images
docker push dragonballregistry.azurecr.io/dragonball-api:latest
docker push dragonballregistry.azurecr.io/dragonball-web:latest
```

### 3. Deploy to Azure Container Apps
```bash
# Create environment
az containerapp env create \
  --name dragonball-env \
  --resource-group dragonball-rg \
  --location eastus \
  --dapr-enabled

# Deploy API service
az containerapp create \
  --name dragonball-api \
  --resource-group dragonball-rg \
  --environment dragonball-env \
  --image dragonballregistry.azurecr.io/dragonball-api:latest \
  --target-port 8080 \
  --ingress external \
  --enable-dapr \
  --dapr-app-id apiservice

# Deploy Web frontend
az containerapp create \
  --name dragonball-web \
  --resource-group dragonball-rg \
  --environment dragonball-env \
  --image dragonballregistry.azurecr.io/dragonball-web:latest \
  --target-port 3000 \
  --ingress external
```

## 🔧 Dapr Configuration

The project includes Dapr components for:

- **State Store**: Azure SQL Database for character persistence
- **Secret Store**: Azure Key Vault for secure configuration
- **Configuration**: Azure App Configuration for dynamic settings
- **Bindings**: Azure Blob Storage for character images

### Component Files:
- `dapr/components/azure-sql.yaml` - SQL Database state store
- `dapr/components/azure-keyvault.yaml` - Key Vault secrets
- `dapr/components/azure-appconfig.yaml` - App Configuration
- `dapr/components/azure-blob-storage.yaml` - Blob Storage bindings

## 📊 Monitoring & Observability

### Health Checks
- **Database Connectivity**: Entity Framework health check
- **Azure Services**: Connection validation
- **Application Health**: Custom health endpoints

### Logging
- **Structured Logging**: JSON formatted logs
- **Azure Monitor**: Application Insights integration
- **Dapr Observability**: Built-in metrics and tracing

### Aspire Dashboard
The Aspire dashboard provides:
- Service discovery and status
- Distributed tracing
- Metrics and telemetry
- Configuration management

## 📁 Project Structure

```
├── DragonBallLibrary.sln                 # Solution file
├── DragonBallLibrary.AppHost/            # Aspire orchestration
│   ├── AppHost.cs                        # Service definitions
│   └── appsettings.json                  # Configuration
├── DragonBallLibrary.ApiService/         # .NET Web API
│   ├── Data/                            # Entity Framework context
│   ├── Services/                        # Business services
│   ├── Program.cs                       # API startup
│   └── Dockerfile                       # Container config
├── DragonBallLibrary.Web/               # React frontend
│   ├── src/
│   │   ├── components/                  # React components
│   │   ├── services/                    # API services
│   │   ├── types/                       # TypeScript types
│   │   └── App.tsx                      # Main component
│   ├── package.json                     # Dependencies
│   └── Dockerfile                       # Container config
├── DragonBallLibrary.ServiceDefaults/   # Shared configuration
├── dapr/                                # Dapr components
│   └── components/                      # Component definitions
└── deploy/                              # Deployment configs
    └── azure-container-apps/            # Azure Container Apps YAML
```

## 🔒 Security

**Important:** This repository contains template configuration files with placeholder values for sensitive data. Before deploying or running the application, you must replace these placeholders with your actual credentials.

⚠️ **For Repository Maintainers:** If you're preparing to make this repository public and it previously contained sensitive data, you must clean the git history first. See [GIT_HISTORY_CLEANUP.md](GIT_HISTORY_CLEANUP.md) for instructions on removing sensitive data from commit history using BFG Repo-Cleaner or git-filter-repo.

Please see [SECURITY.md](SECURITY.md) for detailed information on:
- How to properly configure credentials
- Files that require your attention
- Best practices for secret management
- Azure Key Vault integration
- Git history cleanup (for maintainers)

**Never commit real credentials or secrets to the repository.**

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/new-character`)
3. Make your changes
4. Run tests and ensure build passes
5. Commit changes (`git commit -am 'Add new character feature'`)
6. Push to branch (`git push origin feature/new-character`)
7. Create a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Dragon Ball** franchise by Akira Toriyama
- **Microsoft .NET Aspire** team
- **Azure Container Apps** team
- **Dapr** community
- **React.js** community

---

*Built with ❤️ for the Dragon Ball community and modern cloud-native development*