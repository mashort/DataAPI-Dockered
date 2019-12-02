# DataAPI-Dockered

This repo is to explore some different options for integrating data storage with a (Docker) containerized ASP.NET Core API.

## DataAPI-AzureSQL Branch
This branch consists of an ASP.NET Core API project running inside a Docker container using Entity Framework Core to persist data to an Azure SQL database that is outside of the container.

### Running the solution
A SQL Server instance on Azure is required to run this solution.

In Visual Studio (2019):
- Clean & Rebuild the solution
- Open the Package Manager Console and run this EF Core command `add-migration initSqlAz99`. This creates an EF Core migration (Migrations folder in the solution)
- In Debug mode, run in the Docker profile
  - Running the solution for the first time in the Docker profile will create an Azure SQL database that is higher than the *Basic* pricing tier. Therefore, after initial creation, go into the Azure Portal and configure the database to your needs.

### Notes
- Docker environment variables are used (to avoid baking in appsettings file configuration values into the Docker image)
- SQL Server LocalDB can be used for testing out the app "*on metal*" (e.g. when debugging against the Kestrel / IIS servers)
  - A connection string to SQL Server LocalDB, called *MagsConnectionMssql*, exists in appsettings.Development.json. ASP.NET Core will default to this file when running in Visual Studio.
- To be able to read both appsettings.Development.json connection string at design time and the environment variable provided by Dockerfile at run time, the following syntax needs to be used:
  - In **Startup.cs**
  ```csharp
services.AddDbContext<MagContext>(options =>
  options.UseSqlServer(
    Configuration["ConnectionStrings:MagsConnectionMssql"])); 
  ```
  - In **Dockerfile**
  ```
ENV ConnectionStrings__MagsConnectionMssql="Server=...”
  ```
    - **Note that a double-underscore is used here as the key/value delimiter - see [Key-per-file Configuration Provider](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?tabs=basicconfiguration&view=aspnetcore-2.2#key-per-file-configuration-provider)**
- Placeholders for secrets are used in the connection string environment variable in the **Dockerfile**...
```
ENV ConnectionStrings__MagsConnectionMssql="Server=ENSRV; ...”
```
...and then in **Startup.cs** are replaced with values from environment variables, which are read from a **.env** file:
```csharp
var config = new StringBuilder (Configuration["ConnectionStrings:MagsConnectionMssql"]);
string conn = config.Replace("ENVSRV", Configuration["DB_Server"])
                    .Replace("ENVID", Configuration["DB_UserId"])
                    .Replace("ENVPW", Configuration["DB_PW"])
                    .ToString();
services.AddDbContext<MagContext>(options =>
    options.UseSqlServer(conn));
```
- **docker-compose.yml** is used to keep secrets out of the Dockerfile and out of the API container image. Secrets are passed in as environment variables when the container instance is starting up...
```
services:
  dataapi:
    image: ${DOCKER_REGISTRY-}dataapi
    build:
      context: .
      dockerfile: DataAPI/Dockerfile
    environment:
      - DB_Server
      - DB_UserId
      - DB_PW
```
...By default, docker-compose will read a file called ".env." (there’s no base to the file name, just ".env.") to get the values for these variables.
- **.env** file is used to keep the values of the environment variables that docker-compose will read and pass in to the running container
```
DB_Server=tcp:myserver ...
DB_UserId=mysuerid
DB_PW=mypassword
```
  - This file should be added to the docker-compose folder at the solution level
  - This file is added to **.gitignore** to ensure that the secret values don't get pushed to a public source control repository by mistake.