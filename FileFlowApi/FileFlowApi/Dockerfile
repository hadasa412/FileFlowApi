# שלב 1: בנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# העתק את קבצי הפרויקט
COPY core/*.csproj ./core/
COPY data/*.csproj ./data/
COPY FileFlowApi/*.csproj ./FileFlowApi/

# שחזור התלויות
RUN dotnet restore ./FileFlowApi/FileFlowApi.csproj

# העתק את כל הקבצים (שיהיה אפשר לבנות הכול)
COPY . .

# בניית הפרויקט
RUN dotnet publish ./FileFlowApi/FileFlowApi.csproj -c Release -o /app/publish

# שלב 2: הרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# הפורט שאת/ה משתמש/ת בו ב-launchSettings.json (למשל 5000 או 8080)
EXPOSE 8080

ENTRYPOINT ["dotnet", "FileFlowApi.dll"]
