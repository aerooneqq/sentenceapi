FROM mcr.microsoft.com/dotnet/core/sdk AS base
WORKDIR /app

EXPOSE 6000 6001

FROM mcr.microsoft.com/dotnet/core/sdk AS build
WORKDIR /src

COPY ./ /app/DocumentsAPI/
COPY ./log /app/DocumentsAPI/
RUN dotnet restore /app/DocumentsAPI/DocumentsAPI.csproj
RUN dotnet build /app/DocumentsAPI/DocumentsAPI.csproj -c Release -o /app/DocumentsAPI/out

FROM build AS publish 
RUN dotnet publish /app/DocumentsAPI/DocumentsAPI.csproj -c Release -o /app/DocumentsAPIRelease

FROM base AS final
WORKDIR /app/DocumentsAPIRelease

COPY --from=publish /app/DocumentsAPIRelease ./
ENTRYPOINT ["dotnet", "DocumentsAPI.dll"]