#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["WorkerServiceApp/WorkerServiceApp.csproj", "WorkerServiceApp/"]
RUN dotnet restore "WorkerServiceApp/WorkerServiceApp.csproj"
COPY . .
WORKDIR "/src/WorkerServiceApp"
RUN dotnet build "WorkerServiceApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkerServiceApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkerServiceApp.dll"]