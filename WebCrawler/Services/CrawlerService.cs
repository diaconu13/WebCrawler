using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebCrawler.Common;
using WebCrawler.Helpers;

namespace WebCrawler.Services
{
    public class CrawlerService : ICrawlerService
    {
        private readonly IPersistenceService _persistenceService;
        private readonly Commands _commands;

        public CrawlerService(IPersistenceService persistenceService, Commands commands)
        {
            _persistenceService = persistenceService;
            _commands = commands;
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

                    // in order for the paths to be corrected as absolute this should be done before _persistenceService.PersistData so data will be correctly persisted once
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
                    if (!HasAbsoluteUrl(reference) && IsNotNavigationLink(reference))//prevent same \
                    {
                        //SanitizeUrl(reference);
                        //try to combine and get one nice absolute url
                        var uri = SanitizeUri(reference, url);
                        await GetUrlContents(uri);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Uri SanitizeUri(References reference, Uri url)
        {
                string absoluteUri;
            if (HasAbsoluteUrl(reference))
            {
                absoluteUri = url.Scheme + "://" + url.Host + url.AbsolutePath;
            }
            else
            {
                absoluteUri = url.Scheme + "://" + url.Host + reference.Url.OriginalString;
            }
           
            //absoluteUri = absoluteUri.Remove(absoluteUri.LastIndexOf("//"), 1);
            return new Uri(absoluteUri, UriKind.RelativeOrAbsolute);
        }

        private bool IsNotNavigationLink(References reference)
        {
            return !reference.Url.OriginalString.Contains("#");
        }

        private static void SanitizeUrl(References reference)
        {
            if (reference.Url.OriginalString.StartsWith("/"))
            {
                reference.Url = new Uri(reference.Url.OriginalString.Remove(0, 1));
            }
        }

        private bool HasAbsoluteUrl(References reference)
        {
            return reference.Url.IsAbsoluteUri && reference.Url.AbsolutePath.Length > 1;
        }
    }
}
