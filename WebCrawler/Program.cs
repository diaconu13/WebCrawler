using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace WebCrawler
{
    class Program
    {
        private int _deepness = 0;
        private int _allowedDeepthness = 0;
        
        //todo try to fins a recursive solution

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

            GetUrlContents(uri);

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        private static void LoadResources(string pageContent)
        {
            var resultRows = pageContent.Split('\n');

            var referencesList = GetAllReferences(resultRows);

            referencesList.ForEach(LoadResource);
        }

        private static void LoadResource(References reference)
        {
            if (reference.ReferenceType == ReferenceTypeEnum.Absolute)
            {
                GetUrlContents(reference.Url);
            }
        }

        private static async void GetUrlContents(Uri uri)
        {
            HttpClientHandler httpHandler = new HttpClientHandler
            { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            using (var client = new HttpClient(httpHandler))
            {
                client.BaseAddress = uri;

                HttpResponseMessage response = await client.GetAsync("/");
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();

                EnsureDirectoryExists(uri);

                File.WriteAllText(uri.DnsSafeHost + "/index.html", result);
               
                LoadResources(result);

            }
        }

        /// <summary>
        /// For now relative references are only urls in src and href that does not start with http... in them
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static References[] GetRelativeReferences(string[] result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// For now external references are only urls in src and href that contains http, https. ftp or any other format is not supported for now.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static List<References> GetAllReferences(string[] result)
        {
            const string src = " src=";
            const string href = " href=";

            //this could be done simpler with arrays, but is done like this to demonstrate that i know about IEnumerable and above :)
            IEnumerable<List<References>> allReferences = result.Select(FindReference);

            List<References> flatList = new List<References>();
            foreach (List<References> listsOfReference in allReferences)
            {
                if (listsOfReference.Any())
                {
                    flatList.AddRange(listsOfReference);
                }
            }

            return flatList;
        }

        private static List<References> FindReference(string row)
        {
            List<References> references = new List<References>();
            if (!ElementHasReference(row)) return references;

            var sources = row.Split(' ').ToList().Select(s => s.Split('=')).ToList();
            var itemsWithReferences = sources.Find(l => l.Any(s => s.Contains("href") || s.Contains("src")));
            if (itemsWithReferences.Length % 2 == 0)
            {
                var name = itemsWithReferences[0];
                var url = itemsWithReferences[1].Split('\"')[1];

                try
                {
                    references.Add(new References(new Uri(url, UriKind.RelativeOrAbsolute), name));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return references;
        }

        private static void EnsureDirectoryExists(Uri uri)
        {
            if (Directory.Exists(uri.DnsSafeHost)) return;

            var dir = Directory.CreateDirectory(uri.DnsSafeHost);
            Console.WriteLine("Created directory " + dir.FullName);
        }

        private static Boolean ElementHasReference(string element)
        {
            return element.Length > 0 && element.Contains(" href=") || element.Contains(" src=");
        }

        private static Boolean IsRelativeUrl(string url)
        {
            return url.StartsWith("http");
        }
    }
}