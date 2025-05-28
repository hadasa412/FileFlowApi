# שלב בסיס להרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# שלב הבנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# העתקת קבצי csproj בלבד
COPY ["FileFlowApi/FileFlowApi.csproj", "FileFlowApi/"]
COPY ["service/service.csproj", "service/"]
COPY ["core/core.csproj", "core/"]

# שחזור תלויות
RUN dotnet restore "FileFlowApi/FileFlowApi.csproj"

# העתקת כל הקבצים
COPY . .

# בנייה
WORKDIR "/src/FileFlowApi"
RUN dotnet build "FileFlowApi.csproj" -c Release -o /app/build

# שלב הפרסום
FROM build AS publish
RUN dotnet publish "FileFlowApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# שלב הסופי להרצה
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileFlowApi.dll"]
