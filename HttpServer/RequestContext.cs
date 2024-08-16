using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace HttpServer
{
    public class RequestContext
    {
        private readonly StringReader _httpContent;
        private string _method;
        private string _fullUri;
        private IDictionary<string, string> _headers;
        private StringBuilder _body = new StringBuilder();
        private int _bodyLength = 0;

        public string Method
        {
            get { return _method; }
        }
        public string FullUri
        {
            get { return _fullUri; }
        }
        public IDictionary<string, string> Headers
        {
            get { return _headers; }
        }
        public int ContentLength { get; private set; }
        public string Body
        {
            get { return _body.ToString(); }
        }

        public bool IsBodyReceiveDone
        {
            get
            {
                return _bodyLength == ContentLength ? true : false;
            }
        }

        public RequestContext(string httpContent)
        {
            _httpContent = new StringReader(httpContent);
            _headers = new Dictionary<string, string>();
            RequestContextHandle();
        }
        private void RequestContextHandle()
        {
            var isBody = false;
            string line = _httpContent.ReadLine();
            GetMethods(line);
            while ((line = _httpContent.ReadLine()) != null)
            {
                if (isBody == false)
                {
                    if (line == "")
                    {
                        _body.Append(_httpContent.ReadToEnd());
                        continue;
                    }
                    //headers
                    GetHeaders(line);
                }
            }
            if (_body.Length > 0)
            {
                _bodyLength += Encoding.UTF8.GetByteCount(_body.ToString());
            }
        }
        private void GetMethods(string line)
        {
            var titles = line.Split(' ');
            _method = titles[0];
            _fullUri = titles[1];
        }
        private void GetHeaders(string line)
        {
            var headers = line.Split(':');
            _headers.Add(headers[0], headers[1]);
            if (string.Compare(headers[0], "Content-Length", true) == 0)
            {
                if (int.TryParse(headers[1], out int re))
                { ContentLength = re; }

            }
        }

        public void AppendBody(string body,int len)
        {
            _bodyLength += len;
            _body.Append(body);
        }
    }
}
