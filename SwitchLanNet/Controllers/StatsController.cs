using Microsoft.AspNetCore.Mvc;
using Switch_Lan_Play_Modified_by_omidRR.SwitchLan;
using System;
using System.Linq;
using System.Net;

namespace Switch_Lan_Play_Modified_by_omidRR.Controllers
{
    [Route("/")]
    [ApiController]
    public class StatsController : Controller
    {
        private SLPServer _slp;

        public StatsController(SLPServer slp)
        {
            _slp = slp;
        }

        [HttpGet]
        public IActionResult OnGet()
        {
            try
            {
                var data = _slp?.TestData;

                if (data == null)
                {
                    return Json(new { Success = false, Error = "Failed to retrieve server stats." });
                }

                var hostInfo = GetHostInfo();

                // Mask each IP address
                var maskedUserClientIPs = _slp.clientippptest.Select(MaskIpAddress).ToList();

                return Json(new
                {
                    SpeedStats = new
                    {
                        UploadSpeed = $"{data.Upload} Bytes/s",
                        DownloadSpeed = $"{data.Download} Bytes/s"
                    },
                    ClientCount = _slp.ClientCount,
                    YourInfo = hostInfo,
                    UserClientIP = maskedUserClientIPs,
                });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Error = ex.Message });
            }
        }

        private static object GetHostInfo()
        {
            try
            {
                var addressList = Dns.GetHostEntry(Dns.GetHostName());
                var allAddresses = addressList.AddressList.Select(x => x.ToString().Trim());
                var hostname = addressList.HostName.Trim();

                return new
                {
                    AllHosts = allAddresses,
                    Hostname = hostname
                };
            }
            catch (Exception e)
            {
                return new
                {
                };
            }
        }

        // This is the function to mask IP
        private static string MaskIpAddress(string ipAddress)
        {
            var parts = ipAddress.Split(':');
            var ipParts = parts[0].Split('.');
            if (ipParts.Length == 4)
            {
                return $"{ipParts[0]}.{ipParts[1]}.{ipParts[2]}.*:{parts[1]}";
            }
            return ipAddress;
        }
    }
}