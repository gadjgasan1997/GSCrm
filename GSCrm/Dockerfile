#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 1433

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["GSCrm/GSCrm.csproj", "GSCrm/"]
RUN dotnet restore "GSCrm/GSCrm.csproj"
COPY . .
WORKDIR "/src/GSCrm"
RUN dotnet build "GSCrm.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GSCrm.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GSCrm.dll"]