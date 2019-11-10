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
        private readonly Dictionary<string, string> _downloadedUris;
        private readonly IPersistenceService _persistenceService;
        private readonly Commands _commands;

        public CrawlerService(IPersistenceService persistenceService, Commands commands)
        {
            _downloadedUris = new Dictionary<string, string>(); ;
            _persistenceService = persistenceService;
            _commands = commands;
        }

        public async Task GetUrlContents(Uri uri)
        {
            try
            {
                Console.WriteLine("Downloading from " + uri.ToString());
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
            // is much safer to use uri.ToString() instead of just OriginalString,
            // please have a look at the bottom of this file to see what that is actually means(decompiled ToString from Uri.cs)
            if (_downloadedUris.ContainsKey(reference.Url.ToString()))
            {
                Console.WriteLine("skipped " + reference.Url);
                return;
            }

            _downloadedUris.Add(reference.Url.ToString(), "");

            switch (reference.ReferenceType)
            {
                case ReferenceTypeEnum.Absolute:
                    await GetUrlContents(reference.Url);
                    break;
                case ReferenceTypeEnum.Relative:
                    if (!HasAbsoluteUrl(reference) && IsNotNavigationLink(reference))//prevent same \
                    {
                        Console.WriteLine(url);
                        //SanitizeUrl(reference);
                        //try to combine and get one nice absolute url
                        var uri = SanitizeUri(reference, url);
                        //recursive call to load all resources 
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
/*
Please se Line 62.
decompiled Uri.cs using Resharper navigation to decompiled code, 
this is very very use-full to understand things that are not so obvious
https://www.jetbrains.com/resharper/

[__DynamicallyInvokable]
[SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.Infrastructure)]
public override string ToString()
{
    if (this.m_Syntax == null)
    {
        if (!this.m_iriParsing || !this.InFact(Uri.Flags.HasUnicode))
            return this.OriginalString;
        return this.m_String;
    }
    this.EnsureUriInfo();
        if (this.m_Info.String == null)
    this.m_Info.String = !this.Syntax.IsSimple ? this.GetParts(UriComponents.AbsoluteUri, UriFormat.SafeUnescaped) : this.GetComponentsHelper(UriComponents.AbsoluteUri, (UriFormat)32767);
    return this.m_Info.String;
}
 */
