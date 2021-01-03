# Framework Requirements

* Dotnet core SDK 3.1
* Angular 10
* Node.js 10+
* SQL Server 2017+

# Build
Build scripts use [Nuke.Build](http://www.nuke.build/docs/getting-started/setup.html).

Targets are included for running the database migration scripts, building the net core application,the angular application and running tests. Also the templates for TeamCity and Google Actions are created for running on a build server.

# Applications

## MSDF.DataChecker.WebApi
Dotnet 3.1 Application that host the API version of the application.

* UI has been removed from the API, and the project renamed to WebApi from WebApp.
* Legacy Assemblies are excluded from the project, but has been left in the project for reference until the validation is complete.

## MSDF.DataChecker.JobExecutorDaemon
Dotnet 3.1 Console application that runs the hangfire service as a separate process.

## MSDF.DataChecker.UI
Angular 10 application for the UI.

* Legacy UI remains in the solution as a reference, and is in the folder MSDF.DataChecker.ClientApp

# Build Server

TBD

# Docker

TBD

# PostgreSQL

TBD
