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
            DirectoryInfo destination = EnsureDirectoryExists(uri);

            string fileName = CalculateFileName(uri);

            var path = Path.Combine(destination.FullName, fileName);
            File.WriteAllText(path, result);
        }

        private string CalculateFileName(Uri uri)
        {
            string fileName = "index.html";

            if (IsProbablyAFile(uri))// probably is a file
            {
                if (IsAFileInRoot(uri)) // is a file in root
                {
                    // http://www.eloquentix.com/favicon.png
                    if (uri.Segments.Length == 1)
                    {
                        return uri.Segments[1];
                    }
                }

                var lastIndexOf = uri.AbsolutePath.LastIndexOf("/");
                string foldersStructure = uri.AbsolutePath.Remove(lastIndexOf, uri.AbsolutePath.Length - lastIndexOf);

            }

            return fileName;
        }

        private string CalculateFilePath(Uri uri, string result, string fileName)
        {
            // 
            string path;
            if (uri.LocalPath.Length > 1)
            {
                path = uri.DnsSafeHost + uri.LocalPath;
                File.WriteAllText(path, result);
            }
            else
            {
                path = uri.DnsSafeHost + fileName;
            }

            return path;
        }

        private DirectoryInfo EnsureDirectoryExists(Uri uri)
        {
            if (IsProbablyAFile(uri))// probably is a file
            {
                if (IsAFileInRoot(uri)) // is a file in root
                {
                    return EnsureRootDirectory(uri);
                }

                var lastIndexOf = uri.AbsolutePath.LastIndexOf("/");
                string foldersStructure = uri.AbsolutePath.Remove(lastIndexOf, uri.AbsolutePath.Length - lastIndexOf);
                return Directory.CreateDirectory(uri.DnsSafeHost + foldersStructure);
            }

            if (!IsProbablyAFile(uri) && !IsAFileInRoot(uri))
            {
                //AbsoluteUri = "http://www.eloquentix.com/case_studies_and_clients/"
                // http://www.eloquentix.com/jobs"
                // this is a link to a html page

                string folderName = uri.Segments[1];
                var info = new DirectoryInfo(uri.DnsSafeHost);

                string path = Path.Combine(info.FullName, folderName);

                return Directory.CreateDirectory(path);

            }
            return EnsureRootDirectory(uri);// this is root dir "/"
        }

        private static bool IsAFileInRoot(Uri uri)
        {
            //http://www.eloquentix.com/favicon.png
            return uri.AbsolutePath.IndexOf("/", StringComparison.Ordinal) == 1;
        }

        private static bool IsProbablyAFile(Uri uri)
        {
            //http://www.eloquentix.com/favicon.png
            return uri.AbsolutePath.Contains(".");
        }

        private DirectoryInfo EnsureRootDirectory(Uri uri)
        {
            if (Directory.Exists(uri.DnsSafeHost)) return new DirectoryInfo(uri.DnsSafeHost);
            var dir = Directory.CreateDirectory(uri.DnsSafeHost);
            Console.WriteLine("Created directory " + dir.FullName);
            return dir;
        }
    }
}
