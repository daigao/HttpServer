using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                //slim.Set();
                Console.ReadKey();
                //slim.Reset();
            }
        }
        static async  void Sss(int count)
        {
            for (int i = 0; i < count; i++)
            {
              var tt1= Task.Run(async () =>
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    HttpClient client = new HttpClient();
                    var dfsd = new StringContent("上传的dfjfffffffffffghfg当时的fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddsssssssssssssssssssssssssssssssssssssssssssssggggggggggdddddddddddd内容", Encoding.UTF8);
                    slim.WaitOne();
                    stopwatch.Restart();
                    var re = await client.PostAsync("http://localhost:5000/dd/ddf", dfsd);
                    var text =  await re.Content.ReadAsStringAsync();
                    stopwatch.Stop();
                    Console.WriteLine($"线程{Thread.CurrentThread.ManagedThreadId}，{stopwatch.ElapsedMilliseconds}ms");
                });
                var tt2 = Task.Run(async () =>
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    HttpClient client = new HttpClient();
                    var dfsd = new StringContent("上传的dfjffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddsssssssssssssssssssssssssssssssssssssssssssssggggggggggdddddddddddd内容", Encoding.UTF8);
                    slim.WaitOne();
                    stopwatch.Restart();
                    var re = await client.PostAsync("http://localhost:5000/dd/ddf", dfsd);
                    var text = await re.Content.ReadAsStringAsync();
                    stopwatch.Stop();
                    Console.WriteLine($"线程{Thread.CurrentThread.ManagedThreadId}，{stopwatch.ElapsedMilliseconds}ms");
                });
                await Task.Delay(1000);
                slim.Set();
                await Task.Delay(1000);
                slim.Reset();
            }
        }

    }
}
