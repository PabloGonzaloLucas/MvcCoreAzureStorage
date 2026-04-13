using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MvcCoreAzureStorage.Models;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;
        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }
        
        public async Task<List<string>> GetContainersAsync()
        {
            List<string> containers = new List<string>();
            await foreach(BlobContainerItem container 
                in this.client.GetBlobContainersAsync())
            {
                containers.Add(container.Name);
            }
            return containers;
        }

        public async Task CreateContainerAsync(string containerName)
        {
            await this.client.CreateBlobContainerAsync
                (containerName.ToLower(),
                PublicAccessType.None);
        }

        public async Task DeleteContainerAsync(string containerName)
        {
            await this.client.DeleteBlobContainerAsync(containerName);
        }

        public async Task<List<BlobModel>>
            GetBlobsAsync(string containerName)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            
            List<BlobModel> models = new List<BlobModel>();
            await foreach(BlobItem item in containerClient.GetBlobsAsync())
            {
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);
                BlobModel blob = new BlobModel();
                blob.Nombre = item.Name;
                blob.Url = blobClient.Uri.AbsoluteUri;
                blob.Container = containerName;
                models.Add(blob);
            }
            return models;
        }

        public async Task<Stream> GetBlobStreamAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            // Descargamos la información y obtenemos el stream del fichero
            BlobDownloadInfo response = await blobClient.DownloadAsync();
            return response.Content;
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient blobContainerClient =
                this.client.GetBlobContainerClient(containerName);
            await blobContainerClient.DeleteBlobAsync(blobName);
        }

        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
            
        }
    }

}
