using System;
using System.Threading.Tasks;

namespace WebCrawler.Services
{
    public interface IPersistenceService
    {
        Task PersistData(Uri uri, string result);
    }
}