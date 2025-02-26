# Script to configure my application

# Verify if .NET SDK is installed
if (-Not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "Installing .NET SDK..."
    $dotNetInstallerUrl = "https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-desktop-installer-8.0.100-windows-x64.exe"
    Invoke-WebRequest -Uri $dotNetInstallerUrl -OutFile "dotnet-installer.exe"
    Start-Process -FilePath "dotnet-installer.exe" -ArgumentList "/quiet" -Wait
    Remove-Item "dotnet-installer.exe"
}

# Verify if SQL Server Express is installed
if (-Not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
    Write-Host "Installing SQL Server Express..."
    $sqlServerInstallerUrl = "https://go.microsoft.com/fwlink/?linkid=860240"
    $sqlInstallerPath = "SQLServerInstaller.exe"
    Invoke-WebRequest -Uri $sqlServerInstallerUrl -OutFile $sqlInstallerPath
    Start-Process -FilePath $sqlInstallerPath -ArgumentList "/quiet" -Wait
    Remove-Item $sqlInstallerPath
}
else
{
  # Create or overwrite the appsettings.json
  $appSettingsPath = "ForeignExchange/appsettings.json"
@"
 {
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ForeignExchange;Integrated Security=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_SECRET_KEY",
    "Issuer": "swagger-test",
    "Audience": "swagger-users",
    "ExpirationTime": 3600
  },
  "AlphaVantage": {
    "ApiKey": "YOUR_API_KEY",
    "BaseUrl": "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency={0}&to_currency={1}&apikey={2}"
  },
  "AzureServiceBus": {
    "ConnectionString": "YOUR_AZURE_SERVICE_BUS_CONNECTION_STRING",
    "QueueName": "exchange-rates-queue"
  },
  "EncryptHashing": {
    "SaltSize": 16,
    "IterationCount": 10000,
    "KeySize": 32,
    "Pepper": "YOUR_PEPPER"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
"@ | Set-Content -Path $appSettingsPath

  # Restore project dependencies
  Write-Host "Restoring project dependencies..."
  dotnet restore

  # Remove existing migrations
  Write-Host "Removing existing migrations..."
  dotnet ef migrations remove --project ForeignExchange/ForeignExchange.csproj --startup-project ForeignExchange/ForeignExchange.csproj -y

  # Create a new migration
  Write-Host "Creating new migration..."
  dotnet ef migrations add InitialCreate --project ForeignExchange/ForeignExchange.csproj --startup-project ForeignExchange/ForeignExchange.csproj

  # Update the database with the new migration
  Write-Host "Updating the database..."
  dotnet ef database update --project ForeignExchange/ForeignExchange.csproj --startup-project ForeignExchange/ForeignExchange.csproj

  Write-Host "Script executed successfully! The application is configured."
}