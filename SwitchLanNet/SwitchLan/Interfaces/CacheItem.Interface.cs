using System;
using System.Net;

namespace SwitchLanNet.Interfaces
{
    internal class CacheItem
    {
        public DateTime ExpireAt { get; set; }
        public IPEndPoint RInfo { get; set; }
    }
}
