# ForeignExchange Application

## Overview
The ForeignExchange application is a .NET-based application that provides currency exchange functionalities. This README will guide you through the steps to configure the application using a PowerShell script and get it running on your local machine.

## Prerequisites
Before running the script, ensure that you have the following:

- A Windows operating system.
- Administrative privileges to install software.
- PowerShell available on your system (comes pre-installed on Windows).

## Configuration Steps

### 1. Clone the Repository
 - Clone the ForeignExchange repository to your local machine using Git:

```powershell
git clone <repository-url>
cd ForeignExchange
```

### 2. Open PowerShell
 - Navigate to the directory of your cloned project in PowerShell. You can do this by right-clicking on the folder and selecting “Open PowerShell window here” or using the cd command.

### 3. Edit the Configuration Script
### This script will perform the following actions:

- Check if the .NET SDK is installed. If not, it will download and install the latest version.
- Check if SQL Server Express is installed. If not, it will download and install it.
- Prompt you for the SQL Server instance name if SQL Server is not found on your machine.
- Create or overwrite the appsettings.json file with the appropriate connection string.
- Restore project dependencies.
- Remove any existing migrations.
- Create a new migration.
- Update the database with the new migration.

### 4. Configure the appsettings information defined on the Configuration Script

 - Before running the application, ensure that the appsettings.json file is configured correctly. Here is an example structure of the appsettings.json file:
```json
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
```

- YOUR_SERVER: Replace with the name of your SQL Server instance. Probably will be in place after the script execution, but, you can confirm using the command ();
- YOUR_SECRET_KEY: Use a strong secret key for JWT token generation. Choose a key with more then 32 digits.
- YOUR_API_KEY: Replace with your Alpha Vantage API key. To get your free key (https://www.alphavantage.co/support/#api-key)
- YOUR_AZURE_SERVICE_BUS_CONNECTION_STRING: Add your Azure Service Bus connection string.
- YOUR_PEPPER: Use a strong pepper value for hashing.

### 5. Execute the Configuration Script
 - Execute the configuration script by running:

```powershell
.\setup.ps1
```

### 6. Run the Application
 - After the script completes successfully, you can run the application with the following command:

```powershell
dotnet run --project ForeignExchange/ForeignExchange.csproj
```

## This command will start the application, and you can access it at https://localhost:7195 (or the configured port).

### 7. Testing the Application
- To run tests for the application:=
- Navigate to the ForeignExchange.Tests directory.
- Run the following command:

```powershell
dotnet test
```

## This command will execute the test cases defined in your project.

### 8. Troubleshooting
- If you encounter issues with SQL Server, ensure it is running and that the connection string in appsettings.json is correct.
- Check PowerShell execution policies if you have issues running scripts. You might need to set the policy to allow script execution:

```powershell
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### 9. Limitations
 - Service Bus Integration Issues:
    There have been difficulties in creating an account on Azure Service Bus, which may hinder event-driven features. The module requires testing and correction to ensure reliable communication and message handling.
 
 - Project Structure:
    The current organization of the project uses folders to separate components, which can lead to confusion as the project scales. Without a clear separation of concerns, maintenance and understanding of the codebase may become challenging.

 - Validation Handling:
    The absence of a centralized validation approach may lead to repetitive validation logic across different parts of the application. This could lead to inconsistencies and make it harder to manage changes to validation rules.
 - Error Handling:
    The current implementation might not handle errors effectively, particularly when required fields are missing. This could lead to unhandled exceptions and poor user experience.

 - Authentication Mechanisms:
    The application may be limited to a single method of authentication, which may not suit all users' needs. The lack of multi-factor authentication (MFA) could also pose security risks.

### 10. Possible Improvements
 - Service Bus Testing and Correction:
    Prioritize testing the Service Bus integration to identify and resolve any issues. Consider implementing retry policies and logging to facilitate troubleshooting.

 - Project Layering:
    Refactor the project structure to use layers (e.g., Presentation, Application, Domain, Infrastructure) instead of just folders. This can enhance clarity and modularity, making it easier to manage dependencies and maintain the code.

 - Implement FluentValidation:
    Introduce FluentValidation to streamline the validation process. This would help reduce repetitive code, centralize validation logic, and provide more expressive error messages.

 - Global Error Handling:
    Implement a global error handler (middleware) to catch exceptions and handle errors in a user-friendly manner. This should include returning appropriate HTTP status codes and messages for validation errors and other issues.

 - Enhanced Authentication Options:
    Explore adding alternative authentication methods, such as OAuth or OpenID Connect, to offer users more flexibility. Additionally, implement multi-factor authentication (2FA) to improve security.

 - Improved Logging and Monitoring:
    Enhance logging to capture critical information about application behavior and potential issues. Implementing monitoring solutions can provide insights into the application's performance and health.

    By addressing these limitations and implementing the suggested improvements, the ForeignExchange application can become more robust, maintainable, and user-friendly.