#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["LinuxHydraPartitionController.Api.WebHost/LinuxHydraPartitionController.Api.WebHost.csproj", "LinuxHydraPartitionController.Api.WebHost/"]
RUN dotnet restore "LinuxHydraPartitionController.Api.WebHost/LinuxHydraPartitionController.Api.WebHost.csproj"
COPY . .
WORKDIR "/src/LinuxHydraPartitionController.Api.WebHost"
RUN dotnet build "LinuxHydraPartitionController.Api.WebHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LinuxHydraPartitionController.Api.WebHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LinuxHydraPartitionController.Api.WebHost.dll"]