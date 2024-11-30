using Azure.Storage.Blobs;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.Others
{
    public class BlobStorageHelper : IBlobStorageHelper
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobStorageHelper(IConfiguration configuration)
        {
            // Retrieve connection string and container name from appsettings
            var connectionString = configuration.GetValue<string>("AzureBlobStorage:ConnectionString");
            _containerName = configuration.GetValue<string>("AzureBlobStorage:ContainerName");

            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null)
                return string.Empty;

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync();

                var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                var blobClient = containerClient.GetBlobClient(fileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                var headers = new Azure.Storage.Blobs.Models.BlobHttpHeaders
                {
                    CacheControl = "public, max-age=31536000"
                };
                await blobClient.SetHttpHeadersAsync(headers);

                await blobClient.SetMetadataAsync(new Dictionary<string, string>
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                });

                return fileName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<IDictionary<string, string>> GetMetadataAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            var blobProperties = await blobClient.GetPropertiesAsync();

            return blobProperties.Value.Metadata;
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}
