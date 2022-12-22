#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FamilyBudget/Server/FamilyBudget.Server.csproj", "FamilyBudget/Server/"]
COPY ["FamilyBudget/Client/FamilyBudget.Client.csproj", "FamilyBudget/Client/"]
COPY ["FamilyBudget/Shared/FamilyBudget.Shared.csproj", "FamilyBudget/Shared/"]
RUN dotnet restore "FamilyBudget/Server/FamilyBudget.Server.csproj"
COPY . .
WORKDIR "/src/FamilyBudget/Server"
RUN dotnet build "FamilyBudget.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FamilyBudget.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FamilyBudget.Server.dll"]