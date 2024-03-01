using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using Microsoft.AspNetCore.Cors;

namespace ConvertToPDF.Controllers
{
    [ApiController]
    [Route("[controller]")]
  [EnableCors("AllowAll")]
    public class MergePDFsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> MergePDFsAsync([FromBody] MergeRequest request)
        {
            try
            {
                try
                {
                    byte[] pdf1Data = Convert.FromBase64String(request.pdfDataUri);
                    // If no exception is thrown, the Base64 encoding is valid
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Invalid Base64 encoding: {ex.Message}");

                    // Return a response indicating the error to the client
                    return BadRequest(new { Error = "Invalid Base64 encoding" });
                }
                byte[] pdfData = Convert.FromBase64String(request.pdfDataUri);

                // Read the second PDF file as a byte array
                byte[] pdf2Data = await System.IO.File.ReadAllBytesAsync(request.filename2);

                // Handle the PDF data as needed
                var mergedPdf = new PdfDocument();
                using (var pdf1Stream = new MemoryStream(pdfData))
                using (var pdf2Stream = new MemoryStream(pdf2Data))
                {
                    var pdf1 = PdfReader.Open(pdf1Stream, PdfDocumentOpenMode.Import);
                    var pdf2 = PdfReader.Open(pdf2Stream, PdfDocumentOpenMode.Import);

                    foreach (PdfPage page in pdf1.Pages)
                    {
                        mergedPdf.AddPage(page);
                    }

                    foreach (PdfPage page in pdf2.Pages)
                    {
                        mergedPdf.AddPage(page);
                    }
                }

                var outputPath = $"merged_{DateTime.Now.Ticks}.pdf";
                using (var outputStream = System.IO.File.Create(outputPath))
                {
                    mergedPdf.Save(outputStream);
                }

                Console.WriteLine($"Merged PDF saved successfully: {outputPath}");
                return Ok(new { FilePath = outputPath });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error merging PDFs: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Error = "Error merging PDFs" });
            }
        }
    }

    public class MergeRequest
    {
        public string pdfDataUri { get; set; }
        public string filename2 { get; set; }
    }
}
