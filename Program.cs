// See https://aka.ms/new-console-template for more information
using Azure.Storage.Blobs;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

var folderPath = Path.GetTempPath();
var blobUrl = new Uri("");
var pdfFile = "";
var blobContainerClient = new BlobContainerClient(blobUrl);
var blobClient = blobContainerClient.GetBlobClient(pdfFile);
Console.WriteLine($"Downloading {pdfFile} from {blobUrl}/{pdfFile}...}");
var response = blobClient.DownloadStreaming();
Console.WriteLine($"File downloaded successfully");
Console.WriteLine($"Opening document in read mode...");
var reader = new PdfReader(response.Value.Content);
var outPdf = Path.Join(folderPath,pdfFile);
var pdfDoc = new PdfDocument(reader, new PdfWriter(outPdf));
var noOfPages = pdfDoc.GetNumberOfPages();
var font = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont(StandardFonts.HELVETICA));
Console.WriteLine($"Adding watermark to {pdfFile}....");
for (var i = 1; i <= noOfPages; i++)
{
    WaterMarkRestricted(pdfDoc, i, font);
}
Console.WriteLine($"Saving file into {pdfFile}....");
pdfDoc.Close();
Console.WriteLine($"Process finished");


static void WaterMarkRestricted(PdfDocument pdfDoc, int pageNumber, PdfFont font)
{
    var page = pdfDoc.GetPage(pageNumber);
    var size = page.GetPageSize();
    var width = size.GetWidth();
    var height = size.GetHeight();
    var x = width / 2;
    var y = height - 20;

    var overContent = new PdfCanvas(page);
    overContent.SetFillColor(ColorConstants.RED);
    var p = new Paragraph("RESTRICTED").SetFont(font).SetFontSize(30);
    overContent.SaveState();
    var gs1 = new PdfExtGState();
    gs1.SetFillOpacity(0.3f);
    overContent.SetExtGState(gs1);

    new Canvas(overContent, pdfDoc.GetDefaultPageSize()).ShowTextAligned(p, x, y, pageNumber, TextAlignment.CENTER, VerticalAlignment.MIDDLE, 0);
    overContent.RestoreState();
}