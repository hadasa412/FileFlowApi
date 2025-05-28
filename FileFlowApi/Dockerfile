FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# שימי לב לנתיבים - הם מותאמים למבנה שלך
COPY ["FileFlowApi/FileFlowApi.csproj", "FileFlowApi/"]
COPY ["data/data.csproj", "data/"]
COPY ["service/service.csproj", "service/"]

RUN dotnet restore "FileFlowApi/FileFlowApi.csproj"

COPY . .
WORKDIR "/src/FileFlowApi"
RUN dotnet build "FileFlowApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileFlowApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileFlowApi.dll"]
