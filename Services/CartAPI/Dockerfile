#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/CartAPI/CartAPI.csproj", "Services/CartAPI/"]
RUN dotnet restore "Services/CartAPI/CartAPI.csproj"
COPY . .
WORKDIR "/src/Services/CartAPI"
RUN dotnet build "CartAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CartAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CartAPI.dll"]