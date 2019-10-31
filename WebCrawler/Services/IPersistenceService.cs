using System;
using System.Threading.Tasks;

namespace WebCrawler.Services
{
    public interface IPersistenceService
    {
        Task<string> PersistData(Uri uri, string result);
    }
}