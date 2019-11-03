using System;
using System.Threading.Tasks;

namespace WebCrawler.Services
{
    public interface ICrawlerService
    {
        Task DownloadFromUrl(Uri uri);
        string MakeUrlAccessibleFromLocal(string originalRow);
    }
}