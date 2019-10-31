using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebCrawler.Helpers;

namespace WebCrawler.Services
{
    public class CrawlerService : ICrawlerService
    {
        private readonly IPersistenceService _persistenceService;

        public CrawlerService(IPersistenceService persistenceService)
        {
            _persistenceService = persistenceService;
        }

        public async Task GetUrlContents(Uri uri)
        {
            try
            {
                //make sure content is not compressed
                HttpClientHandler httpHandler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };

                using (var client = new HttpClient(httpHandler))
                {
                    client.BaseAddress = uri;

                    HttpResponseMessage response = await client.GetAsync("/");
                    response.EnsureSuccessStatusCode();

                    string result = await response.Content.ReadAsStringAsync();

                    await _persistenceService.PersistData(uri, result);

                    LoadResources(result, uri);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }


        private void LoadResources(string pageContent, Uri url)
        {
            var resultRows = pageContent.Split('\n');

            var referencesList = ElementsHelpers.GetAllReferences(resultRows);
            var loadResourceTasks = new List<Task>();
            referencesList.ForEach(r => loadResourceTasks.Add(LoadResource(r, url)));
            Task.WaitAll(loadResourceTasks.ToArray());
        }

        private async Task LoadResource(References reference, Uri url)
        {
            switch (reference.ReferenceType)
            {
                case ReferenceTypeEnum.Absolute:
                    await GetUrlContents(reference.Url);
                    break;
                case ReferenceTypeEnum.Relative:
                    if (!HasAbsoluteUrl(reference))//prevent same \
                    {
                        //try to combine and get one nice absolute url
                        var uri = new Uri(url.AbsoluteUri + reference.Url);
                        await GetUrlContents(uri);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool HasAbsoluteUrl(References reference)
        {
            return reference.Url.IsAbsoluteUri && reference.Url.AbsolutePath.Length > 1;
        }
    }
}
