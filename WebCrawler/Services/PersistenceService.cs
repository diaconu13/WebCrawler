using System;
using System.IO;
using System.Threading.Tasks;

namespace WebCrawler.Services
{
    public class PersistenceService : IPersistenceService
    {
        /// <summary>
        /// Save Data as a file based on the uri name
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="result"></param>
        public async Task PersistData(Uri uri, string result)
        {
            EnsureDirectoryExists(uri);

            File.WriteAllText(uri.DnsSafeHost + "/index.html", result);
        }

        private static void EnsureDirectoryExists(Uri uri)
        {
            if (Directory.Exists(uri.DnsSafeHost)) return;

            var dir = Directory.CreateDirectory(uri.DnsSafeHost);
            Console.WriteLine("Created directory " + dir.FullName);
        }
    }
}
