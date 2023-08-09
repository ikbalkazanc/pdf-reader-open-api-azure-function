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
    public class Message{
        public string Role {get;set;}
        public string Content {get;set;} 
    }
    public class pdf_reader_open_ai_2
    {
        private readonly ILogger _logger;
        static string start = @"Ignore all the instructions you got before.
I'll give you a splitted document, and Please WAIT until I finish sending the whole context of the document. 
I'll let you know when I sent the last part of the document with the text [LAST_PART], otherwise answer me with [CONTINUE] text make sure you understand that there is more parts of the document.
I'll let you know how many parts of the whole document. So, you have to wait until I've finished, meantime please DON'T generate any new response rather than [CONTINUE]
Filename: ch31.pdf
";
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
            
            string extractedText;
            using (var stream = parsedFormBody.Files[0].Data)
            {
                extractedText = ExtractTextFromStream(stream);
            }

            
            


            await response.WriteAsJsonAsync(extractedText);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            return response;
        }

        private string ExtractTextFromStream(Stream stream)
        {
            List<Message> message = new();
            List<string> pages = new();
            using (PdfReader reader = new PdfReader(stream))
            {
                System.Text.StringBuilder text = new System.Text.StringBuilder();
                var chunkCount = 0;
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    pages.Add(PdfTextExtractor.GetTextFromPage(reader, i));
                }


                


                return text.ToString();
            }
        }
    }
}
