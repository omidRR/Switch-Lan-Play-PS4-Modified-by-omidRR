using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using RestSharp;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SwitchLanNet
{
    public class Program
    {
        internal static SLPServer _slpServer;

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Switch Lan Play Modified by omidRR");
            await RunArgs(args);
        }

        static async Task RunArgs(string[] args)
        {
            Console.WriteLine("\n\nEnter the port, (Press Enter for random port)");
            Console.Write("Port:");
            string port = Console.ReadLine();
            if (port == "")
                port = new Random().Next(80, 23427).ToString();
            var cts = new CancellationTokenSource();


            Console.CancelKeyPress += delegate
            {
                Console.WriteLine("[INFO] Exiting server..");
                cts.Cancel();
            };
            Console.WriteLine("\n\nEnter the IP address, (if the IP is empty, 0.0.0.0 will be replaced)");
            Console.Write("ip:");
            var Address = Console.ReadLine();

            if (Address == "")
                Address = "0.0.0.0";
            //getip

            var url = "http://ipv4.webshare.io/";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Get);
            request.Timeout = 5000;
            RestResponse response = await client.ExecuteAsync(request);
            if (response.IsSuccessful is true)
            {
                Console.WriteLine("\nClientIP: http://" + response.Content + ":" + port);
            }


            // parse IP option
            if (!IPAddress.TryParse(Address, out var ip))
            {
                Console.WriteLine("[ERROR] Invalid IP address provided.");
                return;
            }

            Console.WriteLine($"\n=>SLP Listening on {ip.ToString()}:{port}...");

            _slpServer = new SLPServer(Convert.ToInt32(port), cts);
            _slpServer.Run();

            CreateWebHostBuilder(ip, Convert.ToInt32(port), args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(IPAddress ip, int port, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseUrls($"http://{ip.ToString()}:{port}")
                .UseStartup<Startup>();
    }
}
