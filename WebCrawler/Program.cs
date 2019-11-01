using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WebCrawler.Common;
using WebCrawler.Helpers;
using WebCrawler.Services;


namespace WebCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {

            CommandsInterpreter interpreter = new CommandsInterpreter();
            Commands commands = interpreter.ParseCommand(args);
            
            Uri uri = new Uri(commands.Address);

            if (!uri.IsAbsoluteUri)
            {
                Console.WriteLine("The uri format is not correct. Must be of form http(s)://www.domain.com");
                Console.WriteLine("Nothing was Done!");
                return;
            }

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IPersistenceService, PersistenceService>()
                .AddSingleton(commands)
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