﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebCrawler.Common;

namespace WebCrawler.Services
{
    public class PersistenceService : IPersistenceService
    {
        private readonly Commands _commands;
        private const string DefaultFileName = "index.html";

        public PersistenceService(Commands commands)
        {
            _commands = commands;
        }

        /// <summary>
        /// Save Data as a file based on the uri name
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="result"></param>
        public async Task<string> PersistData(Uri uri, string result)
        {
            DirectoryInfo destination = EnsureDirectoryExists(uri);

            string fileName = CalculateFileName(uri);

            var path = Path.Combine(destination.FullName, fileName);

            //this is te most time consuming operation, since it deals with the file system
            await Task.Run(() => File.WriteAllText(path, result));
            return path;
        }

        private string CalculateFileName(Uri uri)
        {
            return IsProbablyAFile(uri) ? uri.Segments.ToList().Last() : DefaultFileName;
        }

        internal DirectoryInfo EnsureDirectoryExists(Uri uri)
        {
            if (IsProbablyAFile(uri))
            {
                if (IsAFileInRoot(uri))
                {
                    return EnsureRootDirectory(uri);
                }

                var lastIndexOf = uri.AbsolutePath.LastIndexOf("/", StringComparison.Ordinal);
                string foldersStructure = uri.AbsolutePath.Remove(lastIndexOf, uri.AbsolutePath.Length - lastIndexOf);
                return Directory.CreateDirectory(uri.DnsSafeHost + foldersStructure);
            }

            var isNotRoot = uri.AbsolutePath.Length > 1;
            var isAFileInRoot = IsAFileInRoot(uri);

            if (isNotRoot && isAFileInRoot)
            {
                // AbsoluteUri = "http://www.eloquentix.com/case_studies_and_clients/"
                // http://www.eloquentix.com/jobs"
                // this is a link to a html page

                string folderName = uri.Segments.Last();
                var info = new DirectoryInfo(uri.DnsSafeHost);

                string path = Path.Combine(info.FullName, folderName);

                return Directory.CreateDirectory(path);

            }

            return EnsureRootDirectory(uri);// this is root dir "/"
        }

        private static bool IsAFileInRoot(Uri uri)
        {

            //http://www.eloquentix.com/favicon.png not http://www.eloquentix.com/ui/stylesheets/compiled.css
            return uri.AbsolutePath.IndexOf("/", StringComparison.Ordinal) == 0 && uri.Segments.Length <= 2;
        }

        private static bool IsProbablyAFile(Uri uri)
        {
            //http://www.eloquentix.com/favicon.png
            return uri.AbsolutePath.Contains(".");
        }

        private DirectoryInfo EnsureRootDirectory(Uri uri)
        {
            string path = Path.Combine(_commands.Destination, uri.DnsSafeHost);
            if (Directory.Exists(path)) return new DirectoryInfo(uri.DnsSafeHost);
            var dir = Directory.CreateDirectory(path);
            Console.WriteLine("Created directory " + dir.FullName);
            return dir;
        }
    }
}
