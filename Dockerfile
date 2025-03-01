FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

COPY . ./

RUN dotnet restore "ForeignExchange/ForeignExchange.csproj" && \
    dotnet build "ForeignExchange/ForeignExchange.csproj" -c Release -o /app/build && \
    dotnet publish "ForeignExchange/ForeignExchange.csproj" -c Release -o /app/publish

COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

EXPOSE 8080

CMD ["./entrypoint.sh"]