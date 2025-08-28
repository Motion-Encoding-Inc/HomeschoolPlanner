namespace HomeschoolPlanner.Api.Services
{
    public interface IStorageService
    {
        // Return a client-side upload URL (or fake) and a blobPath token we’ll persist.
        Task<(string uploadUrl, string blobPath)> StartUploadAsync(string fileName, string mimeType, long sizeBytes);

        // For real storage you might validate/upload here. Fake does nothing.
        Task ConfirmUploadAsync(Guid subjectId, string blobPath, string fileName, string mimeType, long sizeBytes);
    }

}
