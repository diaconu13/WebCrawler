using System;
using System.Threading.Tasks;

namespace WebCrawler.Services
{
    interface ICrawlerService
    {
        Task GetUrlContents(Uri uri);
    }
}