#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
RUN dotnet --version
WORKDIR /src
COPY ["ChatBot.App/ChatBot.App.csproj", "ChatBot.App/"]
COPY ["ChatBot.Core/ChatBot.Core.csproj", "ChatBot.Core/"]
COPY ["ChatBot.Data/ChatBot.Data.csproj", "ChatBot.Data/"]
RUN dotnet restore "ChatBot.App/ChatBot.App.csproj"
COPY . .
WORKDIR "/src/ChatBot.App"
RUN dotnet build "ChatBot.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatBot.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatBot.App.dll"]