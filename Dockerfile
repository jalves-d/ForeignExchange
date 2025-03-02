FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY . ./

RUN dotnet restore "ForeignExchange/ForeignExchange.csproj"

RUN dotnet build "ForeignExchange/ForeignExchange.csproj" -c Release -o /app/build

RUN dotnet publish "ForeignExchange/ForeignExchange.csproj" -c Release -o /app/publish

ENV DB_HOST=sqlserver
ENV DB_USER=sa
ENV DB_PASS=Your_Strong_Password_123
ENV PATH="$PATH:/root/.dotnet/tools"

RUN apt-get update && apt-get install -y \
    gnupg \
    lsb-release \
    curl && \
    curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg && \
    install -o root -g root -m 644 microsoft.gpg /etc/apt/trusted.gpg.d/ && \
    curl "https://packages.microsoft.com/config/ubuntu/20.04/prod.list" > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y msodbcsql18 mssql-tools18 && \
    rm -rf /var/lib/apt/lists/*

RUN until /opt/mssql-tools/bin/sqlcmd -S "$DB_HOST" -U "$DB_USER" -P "$DB_PASS" -Q "SELECT 1" &> /dev/null; do \
    echo "SQL Server is unavailable - sleeping..."; \
    sleep 5; \
  done

RUN echo "SQL Server is up - continuing..."

RUN dotnet tool install --global dotnet-ef

RUN dotnet ef migrations remove --project ForeignExchange.Infrastructure/ForeignExchange.Infrastructure.csproj --startup-project ForeignExchange/ForeignExchange.csproj -f || true

RUN dotnet ef migrations add InitialCreate --project ForeignExchange.Infrastructure/ForeignExchange.Infrastructure.csproj --startup-project ForeignExchange/ForeignExchange.csproj

EXPOSE 8080