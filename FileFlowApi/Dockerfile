# שלב 1: בנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# העתקה של כל התוכן (כולל csproj) בפעם אחת
COPY . .

# שחזור תלויות
RUN dotnet restore FileFlowApi/FileFlowApi.csproj

# בנייה
RUN dotnet publish FileFlowApi/FileFlowApi.csproj -c Release -o /app/publish

# שלב 2: הרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "FileFlowApi.dll"]
