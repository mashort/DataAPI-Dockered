# DataAPI-Dockered

This repo is to explore some different options for integrating data storage with a (Docker) containerized ASP.NET Core API.

## DataAPI-SQLite Branch
This branch consists of an ASP.NET Core API project running inside a Docker container using Entity Framework Core to persist data.

It runs SQLite (which gets installed when the ASP.NET Core API project gets built within the container) inside the same container as the ASP.NET Core API project.

### Running the solution
In Visual Studio (2019):
- Clean & Rebuild the solution
- Open the Package Manager Console and run this EF Core command `add-migration initSqlite99`
- In Debug mode, run in Docker
