

namespace DragonBallLibrary.ApiService.Services;

public interface IBlobStorageService
{
    Task<string> UploadCharacterImageAsync(string characterName, Stream imageStream);
    Task<string?> GetCharacterImageUrlAsync(string characterName);
    Task<bool> DeleteCharacterImageAsync(string characterName);
}

public class BlobStorageService : IBlobStorageService
{
    private readonly ILogger<BlobStorageService> _logger;
    private readonly IConfiguration _configuration;
    private readonly DaprClient _daprClient;
    private const string StorageComponentName = "azure-blob-storage";
    private const string ContainerName = "characters";

    public BlobStorageService(ILogger<BlobStorageService> logger, IConfiguration configuration, DaprClient daprClient)
    {
        _logger = logger;
        _configuration = configuration;
        _daprClient = daprClient;
    }

    public async Task<string> UploadCharacterImageAsync(string characterName, Stream imageStream)
    {
        try
        {
            var normalizedName = characterName.ToLowerInvariant().Replace(" ", "");
            var fileName = $"{normalizedName}/{normalizedName}.jpg";
            
            _logger.LogInformation("Uploading image for character {CharacterName} to {FileName}", characterName, fileName);
            
            // Convert stream to byte array
            using var memoryStream = new MemoryStream();
            await imageStream.CopyToAsync(memoryStream);
            var imageData = memoryStream.ToArray();
            
            // Use Dapr to upload to Azure Blob Storage
            await _daprClient.InvokeBindingAsync(StorageComponentName, "create", new
            {
                blobName = fileName,
                data = imageData
            });
            
            return GetImageUrl(normalizedName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for character {CharacterName}", characterName);
            throw;
        }
    }

    public async Task<string?> GetCharacterImageUrlAsync(string characterName)
    {
        try
        {
            var normalizedName = characterName.ToLowerInvariant().Replace(" ", "");
            var fileName = $"{normalizedName}/{normalizedName}.jpg";
            
            _logger.LogInformation("Getting image URL for character {CharacterName} with filename {FileName}", characterName, fileName);
            
            // Check if blob exists using Dapr
            try
            {
                await _daprClient.InvokeBindingAsync(StorageComponentName, "get", new
                {
                    blobName = fileName
                });
                
                // If we get here without exception, the blob exists
                return GetImageUrl(normalizedName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Image not found for character {CharacterName}, returning placeholder", characterName);
                // Return a placeholder URL or a default image URL
                return GetImageUrl(normalizedName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting image URL for character {CharacterName}", characterName);
            return null;
        }
    }

    public async Task<bool> DeleteCharacterImageAsync(string characterName)
    {
        try
        {
            var normalizedName = characterName.ToLowerInvariant().Replace(" ", "");
            var fileName = $"{normalizedName}/{normalizedName}.jpg";
            
            _logger.LogInformation("Deleting image for character {CharacterName} with filename {FileName}", characterName, fileName);
            
            await _daprClient.InvokeBindingAsync(StorageComponentName, "delete", new
            {
                blobName = fileName
            });
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image for character {CharacterName}", characterName);
            return false;
        }
    }
    
    private string GetImageUrl(string normalizedName)
    {
        // In a real implementation, this would be the actual blob storage URL
        // For now, we'll use the storage account name from Dapr configuration
        var storageAccount = _configuration["Dapr:StorageAccount"] ?? "dragonballstorage";
        return $"https://{storageAccount}.blob.core.windows.net/{ContainerName}/{normalizedName}/{normalizedName}.jpg";
    }
}