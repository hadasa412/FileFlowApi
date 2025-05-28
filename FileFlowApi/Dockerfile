# שלב 1: בנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# העתקת קבצי הפרויקטים בלבד (לצורך שחזור תלויות)
COPY core/core.csproj core/
COPY data/data.csproj data/
COPY FileFlowApi/FileFlowApi.csproj FileFlowApi/

# שחזור תלויות
RUN dotnet restore FileFlowApi/FileFlowApi.csproj

# העתקת שאר הקבצים
COPY . .

# בנייה (publish)
RUN dotnet publish FileFlowApi/FileFlowApi.csproj -c Release -o /app/publish

# שלב 2: הרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# פורט לפי מה שמוגדר ב־launchSettings.json
EXPOSE 8080

ENTRYPOINT ["dotnet", "FileFlowApi.dll"]
