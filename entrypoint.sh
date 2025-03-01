#!/bin/sh

set -e  # Exit immediately if any command fails

echo "Waiting for SQL Server to be ready..."
# Wait for SQL Server to be ready before proceeding
until /opt/mssql-tools/bin/sqlcmd -S "$DB_HOST" -U "$DB_USER" -P "$DB_PASS" -Q "SELECT 1" &> /dev/null
do
  echo "SQL Server is unavailable - sleeping..."
  sleep 5
done

echo "SQL Server is up - continuing..."

echo "Checking if dotnet-ef is installed..."
if ! dotnet tool list -g | grep -q "dotnet-ef"; then
    echo "Installing dotnet-ef..."
    dotnet tool install --global dotnet-ef
fi

echo "Restoring project dependencies..."
dotnet restore

echo "Removing existing migrations (if any)..."
dotnet ef migrations remove --project ForeignExchange.Infrastructure/ForeignExchange.Infrastructure.csproj --startup-project ForeignExchange/ForeignExchange.csproj -f || true

echo "Creating new migration..."
dotnet ef migrations add InitialCreate --project ForeignExchange.Infrastructure/ForeignExchange.Infrastructure.csproj --startup-project ForeignExchange/ForeignExchange.csproj

echo "Updating the database..."
dotnet ef database update --project ForeignExchange.Infrastructure/ForeignExchange.Infrastructure.csproj --startup-project ForeignExchange/ForeignExchange.csproj

echo "Starting the application..."
exec dotnet ForeignExchange.dll
