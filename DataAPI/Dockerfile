FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["DataAPI/DataAPI.csproj", "DataAPI/"]
RUN dotnet restore "DataAPI/DataAPI.csproj"
COPY . .
WORKDIR "/src/DataAPI"
RUN dotnet build "DataAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataAPI.dll"]