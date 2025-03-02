# ForeignExchange Application

## Overview
The ForeignExchange application is a .NET-based application that provides currency exchange functionalities. This README will guide you through the steps to configure and run the application on your local machine, whether you're using Windows or Linux.

## Prerequisites
Before proceeding, ensure that you have the following installed:

- Docker Desktop: Required for containerization.
- Git: (Optional) Recommended for cloning the repository.

## Configuration Steps

### 1. Clone the Repository
 - Clone the ForeignExchange repository to your local machine using Git or download it as a ZIP file and extract it:

```powershell
git clone <repository-url>
cd ForeignExchange
```

### 2. Configure Environment Variables in docker-compose.yml
 - Navigate to the directory of your cloned project. Edit the docker-compose.yml file and change the required fields.

```yaml
services:
  # ... other services ...
  foreignexchange:
    # ... build and other configs ...
    environment:
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=ForeignExchange;User=sa;Password=Your_Strong_Password_123;TrustServerCertificate=True
      - JwtSettings__SecretKey=YOUR_SECRET_KEY
      - JwtSettings__Issuer=swagger-test
      - JwtSettings__Audience=swagger-users
      - JwtSettings__ExpirationTime=3600
      - AlphaVantage__ApiKey=YOUR_API_KEY
      - AlphaVantage__BaseUrl=https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency={0}&to_currency={1}&apikey={2}
      - AzureServiceBus__ConnectionString=Endpoint=sb://your-servicebus-name.servicebus.windows.net/;SharedAccessKeyName=your-key-name;SharedAccessKey=your-key
      - AzureServiceBus__QueueName=exchange-rates-queue
      - EncryptHashing__SaltSize=16
      - EncryptHashing__IterationCount=10000
      - EncryptHashing__KeySize=32
      - EncryptHashing__Pepper=YOUR_PEPPER
```

- YOUR_SECRET_KEY: Use a strong secret key for JWT token generation. Choose a key with more then 32 digits.
- YOUR_API_KEY: Replace with your Alpha Vantage API key. To get your free key (https://www.alphavantage.co/support/#api-key)
- YOUR_PEPPER: Use a strong pepper value for hashing.

### Some of the database definitions are made in Dockerfile too, the best practice is to store all your keys in a .env file for application security reasons. (It wasn't made in this project to let him the most easiest runnable possible).

### 3. Build and Run the Application with Docker Compose
### Use Docker Compose to build and run the application:

```powershell
docker-compose up --build -d
```

### This command will:
- Build the Docker image using the provided Dockerfile.
- Start the SQL Server and ForeignExchange containers in detached mode (-d).
- Apply database migrations and start the application using the command defined in docker-compose.yml.
- Access the application at http://localhost:8080/swagger/index.html (or the configured port in your Dockerfile).



### 4. Run Tests
## To run tests for the application, execute the following command inside the container:

```powershell
docker-compose exec foreignexchange dotnet test
```

## This command will execute the test cases defined in your project.

### 8. Troubleshooting
- SQL Server Issues: Ensure SQL Server is running and accessible from within the Docker container. Check the Docker Compose logs for any errors.

```powershell
docker-compose logs sqlserver
```

- Application Startup Issues: Check the Docker Compose logs for any errors during application startup.

```powershell
docker-compose logs foreignexchange
```

- Migration Issues: if the migrations fail, review the logs from the foreignexchange container.

### 9. Limitations
 - Service Bus Integration Issues:
    There have been difficulties in creating an account on Azure Service Bus, which may hinder event-driven features. The module requires testing and correction to ensure reliable communication and message handling.

 - Error Handling:
    The current implementation might not handle errors effectively, particularly when required fields are missing. This could lead to unhandled exceptions and poor user experience.

 - Authentication Mechanisms:
    The application may be limited to a single method of authentication, which may not suit all users' needs. The lack of multi-factor authentication (MFA) could also pose security risks.

### 10. Possible Improvements
 - Service Bus Testing and Correction:
    Prioritize testing the Service Bus integration to identify and resolve any issues. Consider implementing retry policies and logging to facilitate troubleshooting.

 - Global Error Handling:
    Implement a global error handler (middleware) to catch exceptions and handle errors in a user-friendly manner. This should include returning appropriate HTTP status codes and messages for validation errors and other issues.

 - Enhanced Authentication Options:
    Explore adding alternative authentication methods, such as OAuth or OpenID Connect, to offer users more flexibility. Additionally, implement multi-factor authentication (2FA) to improve security.

 - Improved Logging and Monitoring:
    Enhance logging to capture critical information about application behavior and potential issues. Implementing monitoring solutions can provide insights into the application's performance and health.

    By addressing these limitations and implementing the suggested improvements, the ForeignExchange application can become more robust, maintainable, and user-friendly.