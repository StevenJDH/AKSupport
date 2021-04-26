#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AKSupport/AKSupport.csproj", "AKSupport/"]
RUN dotnet restore "AKSupport/AKSupport.csproj"
COPY . .
WORKDIR "/src/AKSupport"
RUN dotnet build "AKSupport.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AKSupport.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AKSupport.dll"]