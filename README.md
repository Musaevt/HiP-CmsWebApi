HiPCMS
======
Develop: [![Build Status](https://travis-ci.org/HiP-App/HiP-CmsWebApi.svg?branch=develop)](https://travis-ci.org/HiP-App/HiP-CmsWebApi/)

HiP-CmsWebApi is a content management system which is developed by the project group [History in 
Paderborn](http://is.uni-paderborn.de/fachgebiete/fg-engels/lehre/ss15/hip-app/pg-hip-app.html).
It is developed to fill the system 'History in Paderborn' with data. This is only a REST API with service end points. We 
also develop [client application](https://github.com/HiP-App/HiP-CmsAngularApp) to consume this REST API. The client application is built on AngularJS2. For Authentication and Authorization we are using a Microservice [HiP-Auth](https://github.com/HiP-App/HiP-Auth).

In another team of the project group, an Android app is developed that will 
make the content of HiPCMS accessable to the public. Information about the app 
will be added as soon as it is available.

HiPCMS will replace the original project which was known as [HiPBackend](https://hip.upb.de/).
HiPBackend's code unfortunately was not maintainable anymore and a rewrite was decided. 

See the LICENSE file for licensing information.

See [the graphs page](https://github.com/HiP-App/HiP-CmsWebApi/graphs/contributors) 
for a list of code contributions.

## Technolgies and Requirements:
HiP-CmsWebApi is a REST API built on .NET Core 1.0 with C# 6.0. Below are the requirements needed to build and develop this project,
 * [.NET Core](https://www.microsoft.com/net/core#windows) for windows or Linux.
 * [PostgreSQL](http://www.postgresql.org/download/)
 
## IDE Options
 * Visual Studio 2015 with Update 3 and [NuGet Package Manager](https://www.nuget.org/). 
 * Visual Studio Code with [C# extention](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp).

## Getting started

 * Clone the repository.
 * Create a new file `appsettings.Development.json` at `scr/Api`. (See `src/Api/appsettings.Development.json.example`).
 * Update the new `appsettings.Development.json` file to match your needs.
 * To run using Visual Studio, just start the app with/without debugging.
 * To run through terminal,
  * Navigate to `src\Api`
  * Set Environment Varriable `ASPNETCORE_ENVIRONMENT=Development`
  * Execute `dotnet run`

## How to develop

 * You can [fork](https://help.github.com/articles/fork-a-repo/) or [clone](https://help.github.com/articles/cloning-a-repository/) our repo.
   * To submit patches you should fork and then [create a Pull Request](https://help.github.com/articles/using-pull-requests/)
   * If you are part of the project group, you can create new branches on the main repo as described [in our internal
     Confluence](http://atlassian-hip.cs.upb.de:8090/display/DCS/Conventions+for+git)


## How to test
Navigate to `\test` folder and run `dotnet test`.


## How to submit Defects and Feature Proposals

Please write an email to [hip-app@campus.upb.de](mailto:hip-app@campus.upb.de).

## Documentation

Documentation is currently collected in our [internal Confluence](http://atlassian-hip.cs.upb.de:8090/dashboard.action). If something is missing in 
this README, just [send an email](mailto:hip-app@campus.upb.de).


## Contact

> HiP (History in Paderborn) ist eine Plattform der:
> Universität Paderborn
> Warburger Str. 100
> 33098 Paderborn
> http://www.uni-paderborn.de
> Tel.: +49 5251 60-0

You can also [write an email](mailto:hip-app@campus.upb.de).
