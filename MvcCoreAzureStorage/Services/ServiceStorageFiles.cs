using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageFiles
    {
        private ShareDirectoryClient root;
        public ServiceStorageFiles(IConfiguration configuration)
        {
            string azureKeys = configuration.GetValue<string>("AzureKeys:StorageAccount");
            ShareClient client = new ShareClient(azureKeys, "ejemplo");
            this.root = client.GetRootDirectoryClient();
        }

        public async Task<List<string>> GetFilesAsync()
        {
            List<string> files = new List<string>();
            await foreach (ShareFileItem item in this.root.GetFilesAndDirectoriesAsync())
            {
                files.Add(item.Name);
            }
            return files;
        }

        public async Task<string> ReadFileAsync(string fileName)
        {
            ShareFileClient fileClient = this.root.GetFileClient(fileName);
            ShareFileDownloadInfo data = await fileClient.DownloadAsync();
            Stream stream = data.Content;
            string contenido = "";
            using (StreamReader reader = new StreamReader(stream))
            {
                contenido = await reader.ReadToEndAsync();
            }
            return contenido;
        }

        public async Task UploadFilesAsync(string fileName, Stream stream)
        {
            ShareFileClient fileClient = this.root.GetFileClient(fileName);
            await fileClient.CreateAsync(stream.Length);
            await fileClient.UploadAsync(stream);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            ShareFileClient fileClient = this.root.GetFileClient(fileName);
            await fileClient.DeleteAsync();
        }
    }
}
