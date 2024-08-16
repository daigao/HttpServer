using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class SocketHandle : IDisposable
    {
        private Socket _soclet;
        private byte[] _buffer;
        private RequestContext _requestContext;
        public event Func<RequestContext, ResponseMessage> RequestMessageCallback;
        public SocketHandle(Socket soclet)
        {
            _soclet = soclet;
        }
        public void Start()
        {
            _buffer = new byte[_soclet.ReceiveBufferSize];
            _soclet.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, _soclet);
        }
        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket serverManager = asyncResult.AsyncState as Socket;
                int len = serverManager.EndReceive(asyncResult);
                if (serverManager.Connected == false) return;
                var text = Encoding.UTF8.GetString(_buffer, 0, len);
                if (_requestContext == null)
                {
                    if (string.IsNullOrWhiteSpace(text)) return;
                    _requestContext = new RequestContext(text);
                }
                else
                {
                    _requestContext.AppendBody(text, len);
                }
                if (!_requestContext.IsBodyReceiveDone)
                {
                    _soclet.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, _soclet);
                    return;
                }
                if (string.IsNullOrWhiteSpace(text)) return;
                var respMsg = RequestMessageCallback?.Invoke(_requestContext);
                if (serverManager.Connected == false) return;

                serverManager.BeginSend(respMsg.GetBytes, 0, respMsg.GetBytes.Length, SocketFlags.None, SendCallback, serverManager);
            }
            catch (Exception)
            {
                Dispose();
                Console.WriteLine("you");
            }
        }
        private void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket serverManager = asyncResult.AsyncState as Socket;
                int len = serverManager.EndSend(asyncResult);
                _requestContext = null;
                serverManager.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, serverManager);
            }
            catch (Exception)
            {
                Dispose();
                Console.WriteLine("you111");
            }
        }

        public void Dispose()
        {
            if (_soclet ==null) return;
            _soclet.Disconnect(false);
            _soclet = null;
        }
    }
}
