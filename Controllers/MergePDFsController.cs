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
                
                var pdf1Data = await System.IO.File.ReadAllBytesAsync(request.filename1);
                var pdf2Data = await System.IO.File.ReadAllBytesAsync(request.filename2);

                var mergedPdf = new PdfDocument();
                using (var pdf1 = PdfReader.Open(new MemoryStream(pdf1Data), PdfDocumentOpenMode.Import))
                using (var pdf2 = PdfReader.Open(new MemoryStream(pdf2Data), PdfDocumentOpenMode.Import))
                {
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
        public string filename1 { get; set; }
        public string filename2 { get; set; }
    }
}
