using Microsoft.AspNetCore.Mvc;
using HtmlToPDFCore;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace testpdf.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet("generate")]
    public async Task<IActionResult> GeneratePdf()
    {
        var html = @"
        <html>
            <title>PDF</title>
            <body>
                <h1>Damn, is this really possible???</h1>
                <b>I think so?</b>
            </body>

            <style>
                h1 {
                    color: blue;
                }
                b {
                    color: red;
                }
            </style>
        </html>";

        var htmlToPdf = new HtmlToPDFCore.HtmlToPDF();
        var pdf = htmlToPdf.ReturnPDF(html);

        var awsAccessKeyId = "AKIARIHVQVMLZFD3RI4G";
        var awsSecretAccessKey = "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";
        var awsRegion = RegionEndpoint.USWest2;

        var awsCredentials = new BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey);
        var s3Client = new AmazonS3Client(awsCredentials, awsRegion);
        Stream pdfStream = new MemoryStream(pdf);

        var request = new PutObjectRequest
        {
            BucketName = "cedric-dotnet-webapi-pdf",
            Key = "testpdf2.pdf",
            InputStream = pdfStream,
            ContentType = "application/pdf" // Set the content type accordingly
        };

        try
        {
            var response = await s3Client.PutObjectAsync(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok("File uploaded successfully");
            }
            else
            {
                return StatusCode((int)response.HttpStatusCode, "Error uploading file to S3");
            }

            // return File(pdf, "application/pdf", "testpdf.pdf");
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }

    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 10).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
