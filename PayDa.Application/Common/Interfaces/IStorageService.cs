namespace PayDa.Application.Common.Interfaces;

public interface IStorageService
{
    Task<string> UploadAsync(Stream file, string fileName, string folder, CancellationToken ct = default);
    Task DeleteAsync(string url, CancellationToken ct = default);
}
