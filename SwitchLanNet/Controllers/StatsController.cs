using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;

namespace SwitchLanNet.Controllers
{
    [Route("/")]
    [ApiController]
    public class StatsController : Controller
    {
        SLPServer _slp;

        public StatsController(SLPServer slp)
        {
            _slp = slp;

        }

        [HttpGet]
        public IActionResult OnGet()
        {
            var data = _slp?.TestData;
            if (data == null)
                return Json(new { Success = false, Error = "Failed to retrieve server stats." });

            return Json(new
            {
                Success = true,

                SpeedStats = new
                {
                    UploadSpeed = $"{data.Upload} Bytes/s",
                    DownloadSpeed = $"{data.Download} Bytes/s"
                },

                _slp.ClientCount
            ,
                yourinfo = dd.dx(),

                userclientIP = _slp.clientippptest,
            });

        }

        public class dd
        {
            public static JsonResult dx()
            {
                try
                {

                    var addlist = Dns.GetHostEntry(Dns.GetHostName());
                    //Retrieve client IP address through HttpContext.Connection
                    var allArray = (addlist.AddressList.Select(x => x.ToString().Trim()));
                    var hostname = addlist.HostName.ToString().Trim();

                    return new JsonResult(new
                    {
                        allHost = allArray,
                        Hostname = hostname
                    });
                }
                catch (Exception e)
                {
                    return new JsonResult(new
                    {
                        errormessage = e.Message,
                        InnerException = e.InnerException
                    });
                }

            }
        }
    }
}