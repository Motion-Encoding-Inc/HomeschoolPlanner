using HomeschoolPlanner.Api.Services;
using HomeschoolPlanner.Data;
using Microsoft.AspNetCore.Mvc;

namespace HomeschoolPlanner.Api.Endpoints
{
    public static class AttachmentEndpoints
    {
        public record StartUploadRequest(Guid SubjectId, string FileName, string MimeType, long SizeBytes);
        public record StartUploadResponse(string UploadUrl, string BlobPath);
        public record ConfirmUploadRequest(Guid SubjectId, string BlobPath, string FileName, string MimeType, long SizeBytes);

        public static async Task<IResult> RequestUpload(
            [FromBody] StartUploadRequest r,
            IStorageService storage)
        {
            // TODO: validate mime/size if you want guardrails
            var (url, path) = await storage.StartUploadAsync(r.FileName, r.MimeType, r.SizeBytes);
            return Results.Ok(new StartUploadResponse(url, path));
        }

        public static async Task<IResult> ConfirmUpload(
            [FromBody] ConfirmUploadRequest r,
            AppDbContext db,
            IStorageService storage)
        {
            await storage.ConfirmUploadAsync(r.SubjectId, r.BlobPath, r.FileName, r.MimeType, r.SizeBytes);

            var a = new Attachment
            {
                Id = Guid.NewGuid(),
                SubjectId = r.SubjectId,
                FileName = r.FileName,
                MimeType = r.MimeType,
                SizeBytes = r.SizeBytes,
                BlobPath = r.BlobPath,
                CreatedUtc = DateTime.UtcNow
            };
            db.Attachments.Add(a);
            await db.SaveChangesAsync();

            return Results.Created($"/api/v1/attachments/{a.Id}", a);
        }
    }
}
