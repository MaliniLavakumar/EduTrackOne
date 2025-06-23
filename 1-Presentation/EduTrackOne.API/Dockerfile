# 1-Presentation/EduTrackOne.API/Dockerfile

# Étape 1 : image runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Installer dépendances et wkhtmltopdf Linux
RUN apt-get update \
    && apt-get install -y wget fontconfig libfreetype6 libx11-6 libxcb1 libxext6 libxrender1 libssl-dev \
    && wget https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6.1-3/wkhtmltox_0.12.6.1-3.bookworm_amd64.deb \
    && dpkg -i wkhtmltox_0.12.6.1-3.bookworm_amd64.deb || true \
    && apt-get install -f -y \
    && rm wkhtmltox_0.12.6.1-3.bookworm_amd64.deb \
    && rm -rf /var/lib/apt/lists/*

    # Étape 2 : image SDK pour build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier la solution et les fichiers .csproj pour restaurer
COPY ["EduTrackOne.sln", "./"]
COPY ["1-Presentation/EduTrackOne.API/EduTrackOne.API.csproj", "1-Presentation/EduTrackOne.API/"]
COPY ["2-Application/EduTrackOne.Application/EduTrackOne.Application.csproj", "2-Application/EduTrackOne.Application/"]
COPY ["3-Domain/EduTrackOne.Domain/EduTrackOne.Domain.csproj", "3-Domain/EduTrackOne.Domain/"]
COPY ["4-Infrastructure/EduTrackOne.Infrastructure/EduTrackOne.Infrastructure.csproj", "4-Infrastructure/EduTrackOne.Infrastructure/"]
COPY ["5-Persistence/EduTrackOne.Persistence/EduTrackOne.Persistence.csproj", "5-Persistence/EduTrackOne.Persistence/"]
COPY ["EduTrackOne.ServiceDefaults/EduTrackOne.ServiceDefaults.csproj", "EduTrackOne.ServiceDefaults/"]
COPY ["EduTrackOne.Contracts/EduTrackOne.Contracts.csproj", "EduTrackOne.Contracts/"]
COPY ["EduTrackOne.Tests/EduTrackOne.Tests.csproj", "EduTrackOne.Tests/"]
COPY ["EduTrackOne.AppHost/EduTrackOne.AppHost.csproj", "EduTrackOne.AppHost/"]

# Restaurer tous les packages
RUN dotnet restore "EduTrackOne.sln"

# Copier le reste du code
COPY . .

# Construire le projet API
WORKDIR /src/1-Presentation/EduTrackOne.API
RUN dotnet build "EduTrackOne.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Étape publish
FROM build AS publish
WORKDIR "/src/1-Presentation/EduTrackOne.API"
RUN dotnet publish "EduTrackOne.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Étape finale runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Créer un lien symbolique vers wkhtmltopdf dans wwwroot/Rotativa
RUN if command -v wkhtmltopdf > /dev/null; then \
      mkdir -p /app/wwwroot/Rotativa && \
      ln -sf "$(command -v wkhtmltopdf)" /app/wwwroot/Rotativa/wkhtmltopdf; \
    else \
      echo "Erreur: wkhtmltopdf non trouvé dans PATH" >&2; exit 1; \
    fi

ENTRYPOINT ["dotnet", "EduTrackOne.API.dll"]
