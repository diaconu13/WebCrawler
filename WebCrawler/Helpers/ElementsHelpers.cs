using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using WebCrawler.Common;

namespace WebCrawler.Helpers
{
    static class ElementsHelpers
    {
        public static bool ElementHasReference(string element)
        {
            //make sure the attribute has form " attr="
            return element.Length > 0 && Constants.ReferenceAttributes.Any(a => element.Contains(" " + a + "="));
            //return element.Length > 0 && element.Contains(" href=") || element.Contains(" src=");
        }

        /// <summary>
        /// For now external references are only urls in src and href that contains http, https. ftp or any other format is not supported for now.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static List<References> GetAllReferences(string[] result)
        {
            const string src = " src=";
            const string href = " href=";

            //this could be done simpler with arrays, but is done like this to demonstrate that I know about IEnumerable, Lists and all that :)
            IEnumerable<List<References>> allReferences = result.Select(FindReference);

            List<References> flatList = new List<References>();
            foreach (List<References> listsOfReference in allReferences)
            {
                if (listsOfReference.Any())
                {
                    flatList.AddRange(listsOfReference);
                }
            }

            return flatList.DistinctBy(references => references.Url.OriginalString).ToList();
        }

        private static List<References> FindReference(string row)
        {
            List<References> references = new List<References>();
            if (!ElementHasReference(row)) return references;

            //todo this is not very accurate since it assume that will be only two items in list all the time ... (src || href)="value".Is this wrong ? ;)
            //most of the time is ok ... but still
            //todo here in order for the site to work offline the relative path must be replaced with the absolute local path
            // I am not sure if this is desired ... this should be clarified. 
            // they are two scenarios,
            //      one where the site could be deployed in a web server and the relative paths are ok
            //      or the site case be run locally case where the paths needs to be absolute.
            
            var sources = row.Split(' ').ToList().Select(s => s.Split('=')).ToList();
            var itemsWithReferences = sources.Find(l => l.Any(s => s.Contains("href") || s.Contains("src")));

            if (itemsWithReferences.Length % 2 == 0)
            {
                var name = itemsWithReferences[0];
                var url = itemsWithReferences[1].Split('\"')[1];

                try
                {
                    var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                    references.Add(new References(uri, name,row));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return references;
        }
    }
}
