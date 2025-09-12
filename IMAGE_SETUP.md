# Character Images Setup

This document explains how to set up character images with Azure Blob Storage and Dapr.

## Image Storage Structure

Images are stored in Azure Blob Storage with the following structure:
```
Container: characters
└── {normalized-character-name}/
    └── {normalized-character-name}.jpg
```

Examples:
- `characters/goku/goku.jpg`
- `characters/vegeta/vegeta.jpg` 
- `characters/piccolo/piccolo.jpg`

## Adding Images

### 1. Download Images from Dragon Ball Wiki
- Visit https://dragonball.fandom.com/wiki/Main_Page
- Navigate to character pages
- Download character images (recommended: 200x200px or larger)

### 2. Normalize Character Names
Character folder names should be:
- Lowercase
- No spaces (replace with empty string)
- No special characters

Examples:
- "Goku" → "goku"
- "Master Roshi" → "masterroshi"
- "Android 18" → "android18"

### 3. Upload to Azure Blob Storage
Using Azure CLI:
```bash
# Create container if not exists
az storage container create --name characters --account-name dragonballstorage

# Upload images with proper folder structure  
az storage blob upload \
  --file ./goku.jpg \
  --container-name characters \
  --name goku/goku.jpg \
  --account-name dragonballstorage
```

### 4. Using Dapr for File Management
The application uses Dapr bindings to interact with Azure Blob Storage:

```csharp
// Upload image via Dapr
await _daprClient.InvokeBindingAsync("azure-blob-storage", "create", new
{
    blobName = "goku/goku.jpg",
    data = imageData
});

// Get image URL
var imageUrl = "https://dragonballstorage.blob.core.windows.net/characters/goku/goku.jpg";
```

## Configuration

### Dapr Component (azure-blob-storage.yaml)
```yaml
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: azure-blob-storage
spec:
  type: bindings.azure.blobstorage
  version: v1
  metadata:
  - name: storageAccount
    value: "dragonballstorage"
  - name: container
    value: "characters"
```

### Application Settings
Set the storage account name in configuration:
```json
{
  "Dapr": {
    "StorageAccount": "dragonballstorage"
  }
}
```

## Image Guidelines

- **Format**: JPG recommended
- **Size**: 200x200px minimum, square aspect ratio preferred
- **Quality**: High quality images from official sources
- **Naming**: Use the exact normalized character name

## Fallback Behavior

- If an image fails to load, the `onError` handler hides the image element
- Character cards still display all other information
- No broken image icons are shown