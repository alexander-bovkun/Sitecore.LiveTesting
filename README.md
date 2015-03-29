This framework allows to easily run tests in real ASP .NET environment as well as simplify Sitecore-related functionality testing.
Here is the short description of the most important parts of the solution:
  * Sitecore.LiveTesting - provides the basic infrastructure to host ASP .NET environment out of IIS process and run tests in it. It also allows to issue requests to the tested web application and analyze retrieved responses  
  * Sitecore.LiveTesting.Extensions - contains additional functionality that allows to change Sitecore configuration on the fly, to change and to track Sitecore pipelines execution  
  * Sitecore.LiveTesting.SpecFlowPlugin - simplifies integration with SpecFlow  
  
### Documentation
If you would like to see more detailed description of framework features, please refer to the [wiki](https://github.com/alexander-bovkun/Sitecore.LiveTesting/wiki) page.
### NuGet
This framework is also available in form of NuGet packages:  
  * [Sitecore.LiveTesting](https://www.nuget.org/packages/Sitecore.LiveTesting)
  * [Sitecore.LiveTesting.Extensions](https://www.nuget.org/packages/Sitecore.LiveTesting.Extensions)
  * [Sitecore.LiveTesting.SpecFlowPlugin](https://www.nuget.org/packages/Sitecore.LiveTesting.SpecFlowPlugin)  

### Compilation
In order to build the project, the following assemblies must be placed to the "lib" subdirectory:  
  * Sitecore.Kernel.dll  