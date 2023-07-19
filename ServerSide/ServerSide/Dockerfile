#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
# install System.Drawing native dependencies
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
     && rm -rf /var/lib/apt/lists/*

RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list \
    && echo "ttf-mscorefonts-installer msttcorefonts/accepted-mscorefonts-eula select true" | debconf-set-selections \
    && apt-get update \
    && apt-get install -y --no-install-recommends ttf-mscorefonts-installer fontconfig libgif-dev autoconf libtool automake build-essential gettext libglib2.0-dev libcairo2-dev libtiff-dev libexif-dev libpango1.0-dev git \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/* \
    && fc-cache -f -v
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ServerSide/ServerSide.csproj", "ServerSide/"]
RUN dotnet restore "ServerSide/ServerSide.csproj"
COPY . .
WORKDIR "/src/ServerSide"
RUN dotnet build "ServerSide.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServerSide.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServerSide.dll"]