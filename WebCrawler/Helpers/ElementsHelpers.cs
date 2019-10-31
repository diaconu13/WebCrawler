﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Helpers
{
    static class ElementsHelpers
    {
        public static Boolean ElementHasReference(string element)
        {
            return element.Length > 0 && element.Contains(" href=") || element.Contains(" src=");
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
                    var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                    references.Add(new References(uri, name));
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
