using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Infrastructure.Services;

public class LocalStorageService : IStorageService
{
    private readonly string _uploadsRoot;
    private readonly string _baseUrl;

    public LocalStorageService(IHostEnvironment env, IConfiguration config)
    {
        _uploadsRoot = Path.Combine(env.ContentRootPath, "wwwroot", "uploads");
        _baseUrl = (config["Storage:LocalBaseUrl"] ?? "http://localhost:5196").TrimEnd('/');
    }

    public async Task<string> UploadAsync(Stream file, string fileName, string folder, CancellationToken ct = default)
    {
        var dir = Path.Combine(_uploadsRoot, folder);
        Directory.CreateDirectory(dir);

        var uniqueName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(dir, uniqueName);

        await using var fs = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fs, ct);

        return $"{_baseUrl}/uploads/{folder}/{uniqueName}";
    }

    public Task DeleteAsync(string url, CancellationToken ct = default)
    {
        const string prefix = "/uploads/";
        var uri = new Uri(url);
        var idx = uri.AbsolutePath.IndexOf(prefix, StringComparison.Ordinal);
        if (idx >= 0)
        {
            var relative = uri.AbsolutePath[(idx + prefix.Length)..].Replace('/', Path.DirectorySeparatorChar);
            var filePath = Path.Combine(_uploadsRoot, relative);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        return Task.CompletedTask;
    }
}
