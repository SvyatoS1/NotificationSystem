FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln* .
COPY src/TicketNotificationApi.Domain/*.csproj ./src/TicketNotificationApi.Domain/
COPY src/TicketNotificationApi.Application/*.csproj ./src/TicketNotificationApi.Application/
COPY src/TicketNotificationApi.Infrastructure/*.csproj ./src/TicketNotificationApi.Infrastructure/
COPY src/TicketNotificationApi.Api/*.csproj ./src/TicketNotificationApi.Api/
COPY tests/TicketNotificationApi.Application.Tests/*.csproj ./tests/TicketNotificationApi.Application.Tests/

RUN dotnet restore src/TicketNotificationApi.Api/TicketNotificationApi.Api.csproj

COPY . .
WORKDIR /app/src/TicketNotificationApi.Api
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

EXPOSE 8080 
ENTRYPOINT ["dotnet", "TicketNotificationApi.Api.dll"]