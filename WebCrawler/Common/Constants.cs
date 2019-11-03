using System.Collections.Generic;

namespace WebCrawler.Common
{
    public struct Constants
    {
        //added constant so if we need to change this we just add items
        public static readonly List<string> ReferenceAttributes = new List<string> {"href","src"};
    }
}
