# שלב 1: בנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# יצירת תיקיות והעתקת קבצי הפרויקטים
RUN mkdir -p core data FileFlowApi
COPY core/*.csproj core/
COPY data/*.csproj data/
COPY FileFlowApi/*.csproj FileFlowApi/

# שחזור התלויות
RUN dotnet restore FileFlowApi/FileFlowApi.csproj

# העתקת שאר הקבצים
COPY . .

# בנייה
RUN dotnet publish FileFlowApi/FileFlowApi.csproj -c Release -o /app/publish

# שלב 2: הרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "FileFlowApi.dll"]
