using HomeschoolPlanner.Data;

namespace HomeschoolPlanner.Api.Endpoints
{
    public static class ReportEndpoints
    {
        public static async Task<IResult> WeeklyCsvOrPdf(
            AppDbContext db, Guid learnerId, DateOnly from, string format = "csv")
        {
            // build dataset; render CSV or PDF (QuestPDF)
            var bytes = System.Text.Encoding.UTF8.GetBytes("date,subject,planned,done\n");
            return Results.File(bytes, format == "pdf" ? "application/pdf" : "text/csv",
                fileDownloadName: $"weekly-{learnerId}-{from}.csv");
        }
    }

}
