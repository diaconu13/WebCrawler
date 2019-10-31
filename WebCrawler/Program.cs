using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WebCrawler.Services;


namespace WebCrawler
{
    class Program
    {
        private int _deepness = 0;
        private int _allowedDeepthness = 0;
        private static string _domain = "";

        //todo try to find a recursive solution

        static async Task Main(string[] args)
        {
            _domain = "http://www.eloquentix.com/";
            Uri uri = new Uri(_domain);


            if (!uri.IsAbsoluteUri)
            {
                Console.WriteLine("The uri format is not correct. Must be of form http(s)://www.domain.com");
                return;
            }

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IPersistenceService, PersistenceService>()
                .AddSingleton<ICrawlerService, CrawlerService>()
                .BuildServiceProvider();

            ICrawlerService crawlerService = serviceProvider.GetService<ICrawlerService>();
            await crawlerService.GetUrlContents(uri);

            Console.WriteLine($"Downloading from {uri.Host}...");

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

    }
}