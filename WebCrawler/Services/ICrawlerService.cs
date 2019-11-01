using System;
using System.Threading.Tasks;
using WebCrawler.Helpers;

namespace WebCrawler.Services
{
    interface ICrawlerService
    {
        Task GetUrlContents(Uri uri);
    }
}