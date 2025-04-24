# Consultez https://aka.ms/customizecontainer pour savoir comment personnaliser votre conteneur de débogage et comment Visual Studio utilise ce Dockerfile pour générer vos images afin d’accélérer le débogage.

# Cet index est utilisé lors de l’exécution à partir de VS en mode rapide (par défaut pour la configuration de débogage)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80



# Cette phase est utilisée pour générer le projet de service
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["1-Presentation/EduTrackOne.API/EduTrackOne.API.csproj", "1-Presentation/EduTrackOne.API/"]
COPY ["2-Application/EduTrackOne.Application/EduTrackOne.Application.csproj", "2-Application/EduTrackOne.Application/"]
COPY ["3-Domain/EduTrackOne.Domain/EduTrackOne.Domain.csproj", "3-Domain/EduTrackOne.Domain/"]
COPY ["4-Infrastructure/EduTrackOne.Infrastructure/EduTrackOne.Infrastructure.csproj", "4-Infrastructure/EduTrackOne.Infrastructure/"]
COPY ["5-Persistence/EduTrackOne.Persistence/EduTrackOne.Persistence.csproj", "5-Persistence/EduTrackOne.Persistence/"]
COPY ["EduTrackOne.ServiceDefaults/EduTrackOne.ServiceDefaults.csproj", "EduTrackOne.ServiceDefaults/"]
RUN dotnet restore "./1-Presentation/EduTrackOne.API/EduTrackOne.API.csproj"
COPY . .
WORKDIR "/src/1-Presentation/EduTrackOne.API"
RUN dotnet build "./EduTrackOne.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Cette étape permet de publier le projet de service à copier dans la phase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EduTrackOne.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Cette phase est utilisée en production ou lors de l’exécution à partir de VS en mode normal (par défaut quand la configuration de débogage n’est pas utilisée)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EduTrackOne.API.dll"]