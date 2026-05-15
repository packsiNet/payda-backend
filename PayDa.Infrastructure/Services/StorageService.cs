using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Infrastructure.Services;

public class StorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public StorageService(IConfiguration config)
    {
        _blobServiceClient = new BlobServiceClient(config["Storage:ConnectionString"]);
        _containerName = config["Storage:ContainerName"] ?? "payda";
    }

    public async Task<string> UploadAsync(Stream file, string fileName, string folder, CancellationToken ct = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobName = $"{folder}/{Guid.NewGuid()}_{fileName}";
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(file, overwrite: true, cancellationToken: ct);
        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(string url, CancellationToken ct = default)
    {
        var uri = new Uri(url);
        var blobName = uri.AbsolutePath.TrimStart('/').Substring(_containerName.Length + 1);
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
    }
}
