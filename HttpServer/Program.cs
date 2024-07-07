using System;

namespace HttpServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (HttpServer httpServer = new HttpServer())
            {
                httpServer.Bind(null);
                httpServer.RequestMessageCallback += (context) =>
                {
                    Console.WriteLine($"{context.Method} {context.FullUri}");
                    return new ResponseMessage("你好服务端");
                };
                httpServer.BeginAccept();
                Console.ReadKey();
            }
        }
    }
}
