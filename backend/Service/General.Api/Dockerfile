#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Microservices/UserManagement/UserManagement.Api/UserManagement.Api.csproj", "Microservices/UserManagement/UserManagement.Api/"]
COPY ["Common/Common.Exceptions/Common.Exceptions.csproj", "Common/Common.Exceptions/"]
COPY ["Common/Common.Events/Common.Events.csproj", "Common/Common.Events/"]
COPY ["Microservices/UserManagement/UserManagement.Infrastructure/UserManagement.Infrastructure.csproj", "Microservices/UserManagement/UserManagement.Infrastructure/"]
COPY ["Common/Common.Shared/Common.Shared.csproj", "Common/Common.Shared/"]
COPY ["Microservices/UserManagement/UserManagement.Domain/UserManagement.Domain.csproj", "Microservices/UserManagement/UserManagement.Domain/"]
COPY ["Microservices/UserManagement/UserManagement.Application/UserManagement.Application.csproj", "Microservices/UserManagement/UserManagement.Application/"]
COPY ["Common/Common.Logger/Common.Logging.csproj", "Common/Common.Logger/"]
COPY ["Common/Common.Behaviours/Common.Behaviours.csproj", "Common/Common.Behaviours/"]
COPY ["Common/Common.Settings/Common.Settings.csproj", "Common/Common.Settings/"]
RUN dotnet restore "Microservices/UserManagement/UserManagement.Api/UserManagement.Api.csproj"
COPY . .
WORKDIR "/src/Microservices/UserManagement/UserManagement.Api"
RUN dotnet build "UserManagement.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserManagement.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserManagement.Api.dll"]