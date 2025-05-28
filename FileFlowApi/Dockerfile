# שלב בסיס להרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE $PORT

# שלב בנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# שלב העתקת קבצי csproj בלבד
COPY ["FileFlowApi/FileFlowApi.csproj", "FileFlowApi/"]
COPY ["service/service.csproj", "service/"]
COPY ["core/core.csproj", "core/"]

# שחזור תלויות
RUN dotnet restore "FileFlowApi/FileFlowApi.csproj"

# העתקת שאר קבצי הפרויקט
COPY . .

# בנייה
WORKDIR "/src/FileFlowApi"
RUN dotnet build "FileFlowApi.csproj" -c Release -o /app/build

# שלב פרסום
FROM build AS publish
RUN dotnet publish "FileFlowApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# שלב סופי להרצה
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileFlowApi.dll"]
