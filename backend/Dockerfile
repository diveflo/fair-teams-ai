FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

COPY . .
RUN dotnet publish -c release -o /app --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /App

COPY --from=build /app .
ENTRYPOINT [ "dotnet", "backend.dll" ]

EXPOSE 80