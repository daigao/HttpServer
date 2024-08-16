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
            _soclet.ReceiveTimeout = 60 * 1000;
            _soclet.Bind(_endPoint);
            _soclet.Listen(_listen);
        }
        public void BeginAccept()
        {
            Console.WriteLine("开始监听");
            _soclet.BeginAccept(AcceptCallback, _soclet);
        }
        private void AcceptCallback(IAsyncResult asyncResult)
        {
            Socket server = asyncResult.AsyncState as Socket;
            var serverManager = server.EndAccept(asyncResult);
            var socketHandle = new SocketHandle(serverManager);
            socketHandle.RequestMessageCallback += RequestMessageCallback;
            socketHandle.Start();
            server.BeginAccept(AcceptCallback, server);
        }

        public void Dispose()
        {
            _soclet.Close();
            _soclet.Dispose();
        }
    }
}
