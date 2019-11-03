using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;
using WebCrawler.Common;
using WebCrawler.Helpers;

namespace WebCrawler.Services.Tests
{
    [TestClass()]
    public class PersistenceServiceTests
    {
        private Commands _commands;


        [TestInitialize]
        public void Setup()
        {
            // not sure yet what needs to be prepared before tests run
            this._commands = new Commands()
            {
                Depth = 2,
                AllowExternal = true,
                Address = new Uri("http://www.eloquentix.com/"),
                Destination = @"C:\Users\dan.diaconu\source\repos\WebCrawler\WebCrawlerTests\bin\Debug",
            };
        }

        [TestMethod()]
        public async Task PersistenceService_PersistData_Root_Page_Persistence_Test()
        {
            //Arrange
            IPersistenceService persistenceService = new PersistenceService(_commands);

            //Act
            string path = await persistenceService.PersistData(new Uri("http://www.eloquentix.com/"), "some html data");

            //Assert
            Assert.IsTrue(path.EndsWith(@"WebCrawlerTests\bin\Debug\www.eloquentix.com\index.html"));
        }

        [TestMethod()]
        public async Task PersistenceService_PersistData_Favicon_Local_Persistence_Test()
        {
           
            //Arrange
            IPersistenceService persistenceService = new PersistenceService(_commands);

            //Act
            string path = await persistenceService.PersistData(new Uri("http://www.eloquentix.com/favicon.png"), "some html data");
            
            //Assert
            Assert.IsTrue(path.EndsWith(@"WebCrawlerTests\bin\Debug\www.eloquentix.com\favicon.png"));
        }


        [TestMethod()]
        public async Task PersistenceService_PersistData_CssOnFoldersTree_Local_Persistence_Test()
        {
            //Arrange
            IPersistenceService persistenceService = new PersistenceService(_commands);

            //Act
            string path = await persistenceService.PersistData(new Uri("http://www.eloquentix.com/ui/stylesheets/compiled.css"), "some css data");

            //Assert
            Assert.IsTrue(path.EndsWith(@"WebCrawlerTests\bin\Debug\www.eloquentix.com\ui\stylesheets\compiled.css"));
        }


        [TestMethod()]
        public async Task PersistenceService_PersistData_CaseStudiesAndClientsPagePersistence_Test()
        {
            //Arrange
            IPersistenceService persistenceService = new PersistenceService(_commands);

            //Act
            string path = await persistenceService.PersistData(new Uri("http://www.eloquentix.com/case_studies_and_clients"), "some css data");

            //Assert
            Assert.IsTrue(path.EndsWith(@"WebCrawlerTests\bin\Debug\www.eloquentix.com\case_studies_and_clients\index.html"));
        }


        [TestMethod()]
        public void CommandsInterpreter_ParseCommand_ParseAllCommands_Test()
        {
            //Arrange
            CommandsInterpreter interpreter = new CommandsInterpreter();
            Commands expectedCommands = _commands;
            string stringCommand = "--allowExternal --address:http://www.eloquentix.com/ --destination:C:\\Users\\dan.diaconu\\source\\repos\\WebCrawler\\WebCrawlerTests\\bin\\Debug --depth:2";
            string[] actualCommand = stringCommand.Split(' ');

            //Act
            Commands result = interpreter.ParseCommand(actualCommand);

            //Assert
            Assert.AreEqual(expectedCommands.AllowExternal, result.AllowExternal);
            Assert.AreEqual(expectedCommands.Destination, result.Destination);
            Assert.AreEqual(expectedCommands.Address, result.Address);
            Assert.AreEqual(expectedCommands.Depth, result.Depth);
        }

        [TestCleanup]
        public void TearDown()
        {
            if (Directory.Exists("www.eloquentix.com"))
            {
                Directory.Delete("www.eloquentix.com", true);
            }
        }
    }
}