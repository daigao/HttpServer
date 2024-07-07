using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpServer
{
    public class HttpServer : IDisposable
    {
        private readonly Socket _soclet;
        private byte[] _buffer;
        private int _listen = 10;
        private IPEndPoint _endPoint;
        public event Func<RequestContext, ResponseMessage> RequestMessageCallback;
        public HttpServer()
        {
            _soclet = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _endPoint = new IPEndPoint(IPAddress.Any, 5000);
        }
        public void Bind(IPEndPoint endPoint)
        {
            if (endPoint != null)
            {
                _endPoint = endPoint;
            }
            _soclet.Bind(_endPoint);
            _soclet.Listen(_listen);
        }
        public void BeginAccept()
        {
            _soclet.BeginAccept(AcceptCallback, _soclet);
        }
        private void AcceptCallback(IAsyncResult asyncResult)
        {
            Socket server = asyncResult.AsyncState as Socket;
            var serverManager = server.EndAccept(asyncResult);
            _buffer = new byte[serverManager.ReceiveBufferSize];
            serverManager.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, serverManager);
            server.BeginAccept(AcceptCallback, server);
        }
        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket serverManager = asyncResult.AsyncState as Socket;
                int len = serverManager.EndReceive(asyncResult);
                if (serverManager.Connected == false) return;
                var text = Encoding.UTF8.GetString(_buffer, 0, len);
                if (string.IsNullOrWhiteSpace(text)) return;
                var requestContext = new RequestContext(text);
                var responseMessage = RequestMessageCallback?.Invoke(requestContext);
                if (serverManager.Connected == false) return;
                serverManager.BeginSend(responseMessage.GetBytes, 0, responseMessage.GetBytes.Length, SocketFlags.None, SendCallback, serverManager);
            }
            catch (Exception)
            {
                Console.WriteLine("you");
            }
        }
        private void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket serverManager = asyncResult.AsyncState as Socket;
                int len = serverManager.EndSend(asyncResult);
                serverManager.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, serverManager);
            }
            catch (Exception)
            {
                Console.WriteLine("you111");
            }
        }
        public void Dispose()
        {
            _soclet.Close();
            _soclet.Dispose();
        }
    }
}
