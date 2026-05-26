FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/BookingService.API/Booking.API.csproj", "src/BookingService.API/"]
COPY ["src/BookingService.Application/Booking.Application.csproj", "src/BookingService.Application/"]
COPY ["src/BookingService.Domain/Booking.Domain.csproj", "src/BookingService.Domain/"]
COPY ["src/BookingService.Infrastructure/Booking.Infrastructure.csproj", "src/BookingService.Infrastructure/"]

RUN dotnet restore "src/BookingService.API/Booking.API.csproj"

COPY . .

RUN dotnet build "src/BookingService.API/Booking.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/BookingService.API/Booking.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Booking.API.dll"]
