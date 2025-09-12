#!/bin/bash

# Dragon Ball Library Deployment Script for Azure Container Apps

set -e

echo "🐉 Dragon Ball Library - Azure Container Apps Deployment"
echo "======================================================"

# Configuration
RESOURCE_GROUP="dragonball-rg"
LOCATION="eastus"
CONTAINER_REGISTRY="dragonballregistry"
CONTAINER_ENV="dragonball-env"
API_APP="dragonball-api"
WEB_APP="dragonball-web"

echo "📝 Configuration:"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  Location: $LOCATION"
echo "  Container Registry: $CONTAINER_REGISTRY"
echo "  Container Environment: $CONTAINER_ENV"
echo ""

# Check if Azure CLI is logged in
echo "🔐 Checking Azure CLI authentication..."
if ! az account show &>/dev/null; then
    echo "❌ Not logged into Azure CLI. Please run 'az login' first."
    exit 1
fi

SUBSCRIPTION_ID=$(az account show --query id -o tsv)
echo "✅ Authenticated to subscription: $SUBSCRIPTION_ID"
echo ""

# Create resource group
echo "📦 Creating resource group..."
az group create \
    --name $RESOURCE_GROUP \
    --location $LOCATION \
    --output none

echo "✅ Resource group created: $RESOURCE_GROUP"
echo ""

# Create Azure Container Registry
echo "🏭 Creating Azure Container Registry..."
az acr create \
    --resource-group $RESOURCE_GROUP \
    --name $CONTAINER_REGISTRY \
    --sku Basic \
    --admin-enabled true \
    --output none

echo "✅ Container registry created: $CONTAINER_REGISTRY"

# Get registry credentials
REGISTRY_SERVER=$(az acr show --name $CONTAINER_REGISTRY --resource-group $RESOURCE_GROUP --query loginServer -o tsv)
REGISTRY_USERNAME=$(az acr credential show --name $CONTAINER_REGISTRY --resource-group $RESOURCE_GROUP --query username -o tsv)
REGISTRY_PASSWORD=$(az acr credential show --name $CONTAINER_REGISTRY --resource-group $RESOURCE_GROUP --query passwords[0].value -o tsv)

echo "📋 Registry details:"
echo "  Server: $REGISTRY_SERVER"
echo "  Username: $REGISTRY_USERNAME"
echo ""

# Build and push API image
echo "🔨 Building and pushing API image..."
docker build -f DragonBallLibrary.ApiService/Dockerfile -t $REGISTRY_SERVER/dragonball-api:latest .
docker login $REGISTRY_SERVER -u $REGISTRY_USERNAME -p $REGISTRY_PASSWORD
docker push $REGISTRY_SERVER/dragonball-api:latest

echo "✅ API image pushed to registry"
echo ""

# Build and push Web image
echo "🔨 Building and pushing Web image..."
docker build -f DragonBallLibrary.Web/Dockerfile -t $REGISTRY_SERVER/dragonball-web:latest ./DragonBallLibrary.Web
docker push $REGISTRY_SERVER/dragonball-web:latest

echo "✅ Web image pushed to registry"
echo ""

# Create Container Apps environment
echo "🌐 Creating Container Apps environment..."
az containerapp env create \
    --name $CONTAINER_ENV \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --enable-workload-profiles false \
    --output none

echo "✅ Container Apps environment created: $CONTAINER_ENV"
echo ""

# Deploy API Container App
echo "🚀 Deploying API service..."
az containerapp create \
    --name $API_APP \
    --resource-group $RESOURCE_GROUP \
    --environment $CONTAINER_ENV \
    --image $REGISTRY_SERVER/dragonball-api:latest \
    --target-port 8080 \
    --ingress external \
    --registry-server $REGISTRY_SERVER \
    --registry-username $REGISTRY_USERNAME \
    --registry-password $REGISTRY_PASSWORD \
    --cpu 0.5 \
    --memory 1Gi \
    --min-replicas 1 \
    --max-replicas 10 \
    --env-vars ASPNETCORE_ENVIRONMENT=Production \
    --output none

API_FQDN=$(az containerapp show --name $API_APP --resource-group $RESOURCE_GROUP --query properties.configuration.ingress.fqdn -o tsv)
echo "✅ API service deployed: https://$API_FQDN"
echo ""

# Deploy Web Container App
echo "🚀 Deploying Web frontend..."
az containerapp create \
    --name $WEB_APP \
    --resource-group $RESOURCE_GROUP \
    --environment $CONTAINER_ENV \
    --image $REGISTRY_SERVER/dragonball-web:latest \
    --target-port 3000 \
    --ingress external \
    --registry-server $REGISTRY_SERVER \
    --registry-username $REGISTRY_USERNAME \
    --registry-password $REGISTRY_PASSWORD \
    --cpu 0.25 \
    --memory 0.5Gi \
    --min-replicas 1 \
    --max-replicas 5 \
    --env-vars NODE_ENV=production REACT_APP_API_URL=https://$API_FQDN \
    --output none

WEB_FQDN=$(az containerapp show --name $WEB_APP --resource-group $RESOURCE_GROUP --query properties.configuration.ingress.fqdn -o tsv)
echo "✅ Web frontend deployed: https://$WEB_FQDN"
echo ""

echo "🎉 Deployment completed successfully!"
echo "=================================================="
echo "📱 Application URLs:"
echo "  Frontend: https://$WEB_FQDN"
echo "  API:      https://$API_FQDN"
echo "  Swagger:  https://$API_FQDN/swagger"
echo "  Health:   https://$API_FQDN/health"
echo ""
echo "🔧 Azure Resources Created:"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  Container Registry: $REGISTRY_SERVER"
echo "  Container Environment: $CONTAINER_ENV"
echo "  API Container App: $API_APP"
echo "  Web Container App: $WEB_APP"
echo ""
echo "🐉 Your Dragon Ball Character Library is now live!"
echo "=================================================="