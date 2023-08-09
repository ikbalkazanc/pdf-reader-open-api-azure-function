using System.Net;
using System.Text;
using HttpMultipartParser;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class pdf_reader_open_ai_2
    {
        private readonly ILogger _logger;

        public pdf_reader_open_ai_2(ILoggerFactory loggerFactory)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _logger = loggerFactory.CreateLogger<pdf_reader_open_ai_2>();
        }

        [Function("pdf_reader_open_ai_2")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            var parsedFormBody = await MultipartFormDataParser.ParseAsync(req.Body);


            var a = parsedFormBody.Files[0].Data;


            string extractedText;
            using (var stream = parsedFormBody.Files[0].Data)
            {
                extractedText = ExtractTextFromStream(stream);
            }

            await response.WriteStringAsync(extractedText);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            return response;
        }

        private string ExtractTextFromStream(Stream stream)
        {
            using (PdfReader reader = new PdfReader(stream))
            {
                System.Text.StringBuilder text = new System.Text.StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }

                return text.ToString();
            }
        }
    }
}
