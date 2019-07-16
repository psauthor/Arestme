FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["DesigningApis.csproj", ""]
RUN dotnet restore "DesigningApis.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "DesigningApis.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DesigningApis.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DesigningApis.dll"]
