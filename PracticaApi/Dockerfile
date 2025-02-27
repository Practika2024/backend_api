FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Будувальний етап
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копіюємо .csproj файли для кожної частини проекту
COPY ["PracticaApi/Api/Api.csproj", "PracticaApi/Api/"]
COPY ["PracticaApi/Application/Application.csproj", "PracticaApi/Application/"]
COPY ["PracticaApi/Domain/Domain.csproj", "PracticaApi/Domain/"]
COPY ["PracticaApi/DataAccessLayer/DataAccessLayer.csproj", "PracticaApi/DataAccessLayer/"]

# Відновлюємо залежності
RUN dotnet restore "PracticaApi/Api/Api.csproj"

# Копіюємо всі файли проекту
COPY . .

# Встановлюємо робочий каталог для Api
WORKDIR "/src/PracticaApi/Api"
RUN dotnet build -c Release -o /app/build

# Публікуємо проект
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Остаточний етап
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]