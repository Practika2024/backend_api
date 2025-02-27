FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PracticaApi/Api/Api.csproj", "PracticaApi/Api/"]
COPY ["PracticaApi/Application/Application.csproj", "PracticaApi/Application/"]
COPY ["PracticaApi/Domain/Domain.csproj", "PracticaApi/Domain/"]
COPY ["PracticaApi/DataAccessLayer/DataAccessLayer.csproj", "PracticaApi/DataAccessLayer/"]
RUN dotnet restore "Api/Api.csproj"

COPY . .
WORKDIR "/Api"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]