using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StressTest
{
    internal class Program
    {
        static ManualResetEvent slim = new ManualResetEvent(false);
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("输入创建的并发数");
                var line = Console.ReadLine();
                if (int.TryParse(line, out var count))
                {
                    Sss(count);
                }
                Console.WriteLine("点击键盘开始");
                Console.ReadKey();
                slim.Set();
                Console.ReadKey();
                slim.Reset();
            }
        }
        static void Sss(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Task.Factory.StartNew(async () =>
                {
                    HttpClient client = new HttpClient();
                    slim.WaitOne();
                    var re = await client.PostAsync("http://localhost:5000/dd/ddf", new StringContent("上传的内容", Encoding.UTF8));
                    Console.WriteLine($"线程{Thread.CurrentThread.ManagedThreadId}，{await re.Content.ReadAsStringAsync()}");
                });
            }
        }

    }
}
