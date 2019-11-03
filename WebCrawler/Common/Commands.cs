using System;

namespace WebCrawler.Common
{
    public class Commands
    {
        public int Depth { get; set; }

        public Uri Address { get; set; }

        public bool AllowExternal { get; set; }

        public string Destination { get; set; }
    }
}