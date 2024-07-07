using System.Net;
using System.Text;

namespace HttpServer
{
    public class ResponseMessage
    {
        private string _httpVersion { get; set; }

        public byte[] GetBytes
        {
            get { return GetMessageBytes(); }
        }

        public string Content { get; set; }

        public Encoding Encoding { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public ResponseMessage()
        {
            Init();
        }
        public ResponseMessage(string content)
        {
            Init();
            Content = content;
        }
        private void Init()
        {
            _httpVersion = "HTTP/1.1";
            Encoding = Encoding.UTF8;
            StatusCode = HttpStatusCode.OK;
        }
        private byte[] GetMessageBytes()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{_httpVersion} {(int)StatusCode} {StatusCode}");
            sb.AppendLine($"Content-Type: text/plain;charset={this.Encoding.BodyName}");
            sb.AppendLine($"Content-Length: {Encoding.GetBytes(Content).Length}");
            sb.AppendLine();
            sb.AppendLine(Content);
            return Encoding.GetBytes(sb.ToString());
        }
    }
}
