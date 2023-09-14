# Use the ASP.NET 7.0 runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Restore and build the main project
COPY ["BlogManager.Adapter.Application/BlogManager.Adapter.Application.csproj", "BlogManager.Adapter.Application/"]
RUN dotnet restore "BlogManager.Adapter.Application/BlogManager.Adapter.Application.csproj"

# Restore and build class libraries
COPY ["BlogManager.Adapter.Api/BlogManager.Adapter.Api.csproj", "BlogManager.Adapter.Api/"]
COPY ["BlogManager.Adapter.Logger/BlogManager.Adapter.Logger.csproj", "BlogManager.Adapter.Logger/"]
COPY ["BlogManager.Adapter.PostgreSQL/BlogManager.Adapter.PostgreSQL.csproj", "BlogManager.Adapter.PostgreSQL/"]
COPY ["BlogManager.Adaptor.EventStore/BlogManager.Adaptor.EventStore.csproj", "BlogManager.Adapter.EventStore/"]
COPY ["BlogManager.Core/BlogManager.Core.csproj", "BlogManager.Core/"]



RUN dotnet restore "BlogManager.Adapter.Api/BlogManager.Adapter.Api.csproj"
RUN dotnet restore "BlogManager.Adapter.Logger/BlogManager.Adapter.Logger.csproj"
RUN dotnet restore "BlogManager.Adapter.PostgreSQL/BlogManager.Adapter.PostgreSQL.csproj"
RUN dotnet restore "BlogManager.Adapter.EventStore/BlogManager.Adaptor.EventStore.csproj"
RUN dotnet restore "BlogManager.Core/BlogManager.Core.csproj"

# Copy all source files and build
COPY . .
WORKDIR "/src/BlogManager.Adapter.Application"
RUN dotnet build "BlogManager.Adapter.Application.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlogManager.Adapter.Application.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlogManager.Adapter.Application.dll"]
