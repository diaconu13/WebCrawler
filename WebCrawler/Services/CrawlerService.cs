using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task DownloadFromUrl(Uri uri)
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

                    var absoluteResult = MakeLinksAbsolute(result);

                    await _persistenceService.PersistData(uri, absoluteResult);

                    // in order for the paths to be corrected as absolute this should be done before _persistenceService.PersistData so data will be correctly persisted once
                    LoadResources(result, uri);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private string MakeLinksAbsolute(string result)
        {
            IEnumerable<string> rows = result.Split('\n').Select(MakeUrlAccessibleFromLocal);

            string absoluteResult = string.Join("", rows);
            return absoluteResult;
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
                    await DownloadFromUrl(reference.Url);
                    break;
                case ReferenceTypeEnum.Relative:
                    if (!HasAbsoluteUrl(reference) && IsNotNavigationLink(reference))//prevent same \
                    {
                        //SanitizeUrl(reference);
                        //try to combine and get one nice absolute url
                        var uri = SanitizeUri(reference, url);
                        await DownloadFromUrl(uri);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string MakeUrlAccessibleFromLocal(string originalRow)
        {
            Constants.ReferenceAttributes.ForEach(c =>
            {
                var formattedString = $" {c}=\"";
                originalRow = originalRow.Replace(formattedString, formattedString + _commands.Destination).Replace("/", "\\").Replace("<\\", "</");
            });

            return originalRow;
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
