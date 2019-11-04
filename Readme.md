# WebCrawler

This is a .Net C# Web Crawler made to demonstrate some C# skills
Note: This is not made to be a and to end produc , not yet at least.

## Project requirements

Create C # app consoles that crawl to a website and save it to disk. The main requirements are:

1. You can configure it how deep to navigate
2. Be able to see as much of the locally downloaded website when there is no internet connection
   Do not download the entire Internet - just limit the links below the specified domain as an argument when executing the command
3. The project should contain a README file describing the architecture / structure of the project in English

The theme is free-style - do what you want and what you want in C #, but without using a crawling lib. We want to see how you think about architecture and how you structure the code when you have no restrictions, etc. If you have extra ideas or want to change something because it looks better, go for it.

Bonus points for:

1. Parallel execution
2. Tests

## How much was done

1. The console supports commands that can and as much functionlaity and configuration as nedded
2. Currenly the site html page and references like .css, images, logo, are downloaded and of the related pages.
   Limitations: in order for the links to wrok they must be conversted to abslote kinks. See To do 1.
3. This feature will be supported, the commands exist but is not fully implemented. See To do 2.
4. Work in progress

Bonuses
1. The project uses Async Await and after the first scan is done and all references(links) are optained they are downloaded in parallel. Most of the work is done in a non blocking way.
2. The project has unit Tests, see WebCrawler done with MSUnit Tests, 5 tests for now to test the Commands interpretation and DIsk persistence.

## To do

1. Convert all links to absolute so all site scripts and resources can be loaded from disk.
2. Commands are implemented but functionality nedds to be completed
   By functionlaity it is meant that the dephness is not yet controlled (might download the internet :) ) and the downloaded referecences such as .css and .js is not loaded in index.html page links needs to be absolute to local system.

## Architecture

To be detailed

https://github.com/diaconu13/WebCrawler/blob/master/architecture.png
![Screenshot](architecture.png)

## Getting Started

The project is a console application that will go throw a website and download the page and all related dependencies, scripts and related pages

### Prerequisites

//todo

### Running

Sample command

```
webcrawler.exe --allowExternal --address:http://www.eloquentix.com/ --destination:C:\Users\dan.diaconu\source\repos\WebCrawler\WebCrawler\bin\Debug --depth:2
```

## Running the tests

//todo

### Break down into end to end tests

//todo

### And coding style tests

//todo

## Deployment

//todo

## Built With

//todo

## Contributing

//todo

## Versioning

//todo

## Authors

//todo

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

//todo
