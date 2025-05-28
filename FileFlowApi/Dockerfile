# שלב 1: בנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# העתק את קבצי הפרויקט לפי המבנה
COPY core/*.csproj core/
COPY data/*.csproj data/
COPY FileFlowApi/*.csproj FileFlowApi/

# שחזור התלויות
RUN dotnet restore FileFlowApi/FileFlowApi.csproj

# העתק את שאר קבצי המקור
COPY . .

# בנייה לפרסום
RUN dotnet publish FileFlowApi/FileFlowApi.csproj -c Release -o /app/publish

# שלב 2: הרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# פורט — ודא שהוא תואם למה שמוגדר אצלך ב-Program.cs או ב-launchSettings.json
EXPOSE 8080

ENTRYPOINT ["dotnet", "FileFlowApi.dll"]
