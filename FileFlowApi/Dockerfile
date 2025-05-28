# שלב בסיס להרצת האפליקציה
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# שלב בנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# העתקת קבצי csproj מהפרויקטים
COPY ["FILEFLOWAPI/FILEFLOWAPI.csproj", "FILEFLOWAPI/"]
COPY ["data/data.csproj", "data/"]
COPY ["service/service.csproj", "service/"]
COPY ["core/core.csproj", "core/"]

# שחזור תלויות
RUN dotnet restore "FILEFLOWAPI/FILEFLOWAPI.csproj"

# העתקת שאר הקבצים
COPY . .

# הגדרת תיקייה לבנייה
WORKDIR "/src/FILEFLOWAPI"
RUN dotnet build "FILEFLOWAPI.csproj" -c Release -o /app/build

# שלב פרסום
FROM build AS publish
RUN dotnet publish "FILEFLOWAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# שלב סופי – הרצה
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FILEFLOWAPI.dll"]
