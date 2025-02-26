# Script to configure my application

# Verify if .NET SDK is installed
if (-Not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "Installing .NET SDK using dotnet-install.ps1..."
    $dotNetInstallerUrl = "https://builds.dotnet.microsoft.com/dotnet/scripts/v1/dotnet-install.ps1"
    $dotNetInstallerPath = "dotnet-install.ps1"
    Invoke-WebRequest -Uri $dotNetInstallerUrl -OutFile $dotNetInstallerPath

    # Execute dotnet-install.ps1 and wait for completion
    try {
        & powershell.exe -ExecutionPolicy Bypass -File $dotNetInstallerPath -Channel 8.0
        Write-Host ".NET SDK installation completed."
    }
    catch {
        Write-Error "Error during .NET SDK installation: $($_.Exception.Message)"
    }
    finally {
        Remove-Item $dotNetInstallerPath
    }
}

# Verify if SQL Server Express is installed
if (-Not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
    Write-Host "Installing SQL Server Express..."
    $sqlServerInstallerUrl = "https://download.microsoft.com/download/7/f/8/7f8a9c43-8c8a-4f7c-9f92-83c18d96b681/SQL2019-SSEI-Expr.exe"
    $sqlInstallerPath = "SQLServerInstaller.exe"
    Invoke-WebRequest -Uri $sqlServerInstallerUrl -OutFile $sqlInstallerPath

    # Start the installation with license terms accepted
    Start-Process -FilePath $sqlInstallerPath -ArgumentList "/quiet", "/IAcceptSQLServerLicenseTerms=TRUE" -Wait

    # Add an additional wait time (adjust as needed)
    Start-Sleep -Seconds 60

    # Verify that the SQL Server Express process has finished
    $processName = "SQL2019-SSEI-Expr" # Adjust if necessary
    $process = Get-Process -Name $processName -ErrorAction SilentlyContinue

    if ($process -eq $null) {
        Write-Host "SQL Server Express installation completed."
    } else {
        Write-Warning "SQL Server Express installation may have failed or is still running."
    }

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