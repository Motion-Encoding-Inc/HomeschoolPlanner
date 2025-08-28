using HomeschoolPlanner.Data;

namespace HomeschoolPlanner.Api.Endpoints
{
    public static class AttachmentEndpoints
    {
        public record StartUploadRequest(Guid SubjectId, string FileName, string MimeType, long SizeBytes);
        public record StartUploadResponse(string UploadUrl, string BlobPath);

        public static IResult RequestUpload(StartUploadRequest r, IConfiguration cfg)
        {
            // Validate mime/size here; 100MB cap
            // Generate SAS URL (Azure.Storage.Blobs)
            // Return { uploadUrl, blobPath } for client to PUT/POST directly
            return Results.Ok(new StartUploadResponse(
                UploadUrl: "https://storage/..../sas",
                BlobPath: $"attachments/{Guid.NewGuid()}-{r.FileName}"
            ));
        }

        public static async Task<IResult> ConfirmUpload(AppDbContext db, Attachment a)
        {
            a.Id = Guid.NewGuid();
            db.Attachments.Add(a);
            await db.SaveChangesAsync();
            return Results.Created($"/api/v1/attachments/{a.Id}", a);
        }
    }

}
