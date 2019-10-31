using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace WebCrawler.Services.Tests
{
    [TestClass()]
    public class PersistenceServiceTests
    {
        [TestInitialize]
        public void Setup()
        {

        }

        [TestMethod()]
        public async Task PersistData_Root_Page_Persistence_Test()
        {
            //Arrange
            IPersistenceService persistenceService = new PersistenceService();

            //Act
            string path = await persistenceService.PersistData(new Uri("http://www.eloquentix.com/"), "some html data");

            //Assert
            Assert.IsTrue(path.EndsWith(@"WebCrawlerTests\bin\Debug\www.eloquentix.com\index.html"));
        }

        

        [TestMethod()]
        public async Task PersistData_Favicon_Local_Persistence_Test()
        {
            //Arrange
            IPersistenceService persistenceService = new PersistenceService();

            //Act
            string path = await persistenceService.PersistData(new Uri("http://www.eloquentix.com/favicon.png"), "some html data");
            //C:\\Users\\dan.diaconu\\source\\repos\\WebCrawler\\WebCrawlerTests\\bin\\Debug\\www.eloquentix.com\\favicon.png
            //Assert
            Assert.IsTrue(path.EndsWith(@"WebCrawlerTests\bin\Debug\www.eloquentix.com\favicon.png"));
        }


        [TestMethod()]
        public async Task PersistData_CssOnFoldersTree_Local_Persistence_Test()
        {
            //Arrange
            IPersistenceService persistenceService = new PersistenceService();

            //Act
            string path = await persistenceService.PersistData(new Uri("http://www.eloquentix.com/ui/stylesheets/compiled.css"), "some css data");
            
            //Assert
            Assert.IsTrue(path.EndsWith(@"WebCrawlerTests\bin\Debug\www.eloquentix.com\ui\stylesheets\compiled.css"));
        }


        [TestMethod()]
        public async Task PersistData_CaseStudiesAndClientsPagePersistence_Test()
        {
            //Arrange
            IPersistenceService persistenceService = new PersistenceService();

            //Act
            string path = await persistenceService.PersistData(new Uri("http://www.eloquentix.com/case_studies_and_clients"), "some css data");

            //Assert
            Assert.IsTrue(path.EndsWith(@"WebCrawlerTests\bin\Debug\www.eloquentix.com\case_studies_and_clients\index.html"));
        }
    }
}