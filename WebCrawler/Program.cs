using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace WebCrawler
{
    class Program
    {
        private int _deepness = 0;
        private int _allowedDeepthness = 0;

        static async Task Main(string[] args)
        {
            string domain = "http://www.eloquentix.com/";
            Uri uri = new Uri(domain);
            if (!uri.IsAbsoluteUri)
            {
                Console.WriteLine("The uri format is not correct. Must be of form http(s)://www.domain.com");
                return;
            }

            Console.WriteLine($"Downloading from {uri.Host}...");

            await SaveSiteContents(uri);

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        private static async Task SaveSiteContents(Uri uri)
        {
            HttpClientHandler httpHandler = new HttpClientHandler
            { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            using (var client = new HttpClient(httpHandler))
            {
                client.BaseAddress = uri;

                HttpResponseMessage response = await client.GetAsync("/");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();

                EnsureDirectoryExists(uri);

                File.WriteAllText(uri.DnsSafeHost + "/index.html", result);
                References[] externalReferences = GetExternalReferences(result);
                References[] relativeReferenceses  = GetRelativeReferences(result);
            }
        }

        private static References[] GetRelativeReferences(string result)
        {
            throw new NotImplementedException();
        }

        private static References[] GetExternalReferences(string result)
        {
            throw new NotImplementedException();
        }

        private static void EnsureDirectoryExists(Uri uri)
        {
            if (Directory.Exists(uri.DnsSafeHost)) return;

            var dir = Directory.CreateDirectory(uri.DnsSafeHost);
            Console.WriteLine("Created directory " + dir.FullName);
        }

        private static Boolean ElementHasExternalReference(string element)
        {
            return element.Length > 0 && element.Contains("href");
        }

        private static Boolean IsRelativeUrl(string url)
        {
            return url.StartsWith("http");
        }
    }
}