using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Switch_Lan_Play_Modified_by_omidRR.SwitchLan;
using System.Net.Http;
using Newtonsoft.Json;

namespace Switch_Lan_Play_Modified_by_omidRR
{
    public class Program
    {
        internal static SLPServer _slpServer;

        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Switch Lan Play Modified by omidRR");
                await RunArgs(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public partial class AppInfo
        {
            public string LatestVersion { get; set; }
            public bool IsActive { get; set; }
        }

        public static async Task CheckAppStatus()
        {
            try
            {
                var httpClient = new HttpClient();
                var appInfoJson = await httpClient.GetStringAsync("https://raw.githubusercontent.com/omidRR/Switch-Lan-Play-PS4-Modified-by-omidRR/master/appinfo.json");

                var appInfo = JsonConvert.DeserializeObject<AppInfo>(appInfoJson);

                var currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; // Read version from Assembly
                var latestVersion = new Version(appInfo.LatestVersion);

                if (currentVersion < latestVersion)
                {
                    throw new Exception("Your version is outdated. Please update to the latest version.");
                }

                if (!appInfo.IsActive)
                {
                    throw new Exception("The application is no longer active.");
                }

                // Continue your code here if the version is not outdated and the app is active
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                // You can exit the program here if needed
                Environment.Exit(1);
            }
        }
        static async Task RunArgs(string[] args)
        {
            try
            {
                await CheckAppStatus();

                var portEnv = Environment.GetEnvironmentVariable("port");
                var ipEnv = Environment.GetEnvironmentVariable("ip");

                string port;
                if (!string.IsNullOrEmpty(portEnv))
                {
                    port = portEnv;
                }
                else
                {
                    Console.WriteLine("\n\nEnter the port, (Press Enter for random port)");
                    Console.Write("Port:");
                    port = Console.ReadLine();
                    if (port == "")
                        port = new Random().Next(80, 23427).ToString();
                }

                var cts = new CancellationTokenSource();

                Console.CancelKeyPress += delegate
                {
                    Console.WriteLine("[INFO] Exiting server..");
                    cts.Cancel();
                };

                var Address = string.IsNullOrEmpty(ipEnv) ? "0.0.0.0" : ipEnv;

                try
                {
                    var httpClient = new HttpClient();
                    httpClient.Timeout = TimeSpan.FromMilliseconds(5000);
                    var response = await httpClient.GetStringAsync("http://ipv4.webshare.io/", cts.Token);

                    Console.WriteLine("\nClientIP: http://" + response.Trim() + ":" + port);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Request failed: " + ex.Message);
                }

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(IPAddress ip, int port, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseUrls($"http://{ip.ToString()}:{port}")
                .UseStartup<Startup>();
    }
}