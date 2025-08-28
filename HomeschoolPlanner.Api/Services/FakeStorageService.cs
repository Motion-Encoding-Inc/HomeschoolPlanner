namespace HomeschoolPlanner.Api.Services
{
    public sealed class FakeStorageService : IStorageService
    {
        public Task<(string uploadUrl, string blobPath)> StartUploadAsync(string fileName, string mimeType, long sizeBytes)
        {
            var id = Guid.NewGuid().ToString("n");
            var path = $"attachments/{id}-{fileName}";
            var url = $"https://fake.local/upload/{id}?file={Uri.EscapeDataString(fileName)}";
            return Task.FromResult((url, path));
        }

        public Task ConfirmUploadAsync(Guid subjectId, string blobPath, string fileName, string mimeType, long sizeBytes)
            => Task.CompletedTask;
    }

}
