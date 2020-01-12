FROM mcr.microsoft.com/dotnet/core/sdk AS base
WORKDIR /app

EXPOSE 5000 5001 8080 8018

FROM mcr.microsoft.com/dotnet/core/sdk AS build
WORKDIR /src

COPY ./ /app/SentenceAPI/
COPY ./log /app/SentenceAPI/
RUN dotnet restore /app/SentenceAPI/SentenceAPI.csproj
RUN dotnet build /app/SentenceAPI/SentenceAPI.csproj -c Release -o /app/SentenceAPI/out

FROM build AS publish 
RUN dotnet publish /app/SentenceAPI/SentenceAPI.csproj -c Release -o /app/SentenceAPIRelease

FROM base AS final
WORKDIR /app/SentenceAPIRelease

COPY --from=publish /app/SentenceAPIRelease ./