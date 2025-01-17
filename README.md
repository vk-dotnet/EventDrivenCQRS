Event Driven CQRS API

Overview

This project demonstrates an Event-Driven CQRS architecture using .NET 8 with RabbitMQ as the messaging backbone. The application features Dead Letter Queues (DLQ) for error handling and a retry mechanism to reprocess failed messages.

Features

Event-Driven Architecture

CQRS Design Pattern

RabbitMQ for Message Queuing

Dead Letter Queues (DLQ)

Retry Mechanism

Requirements

.NET 8 SDK

RabbitMQ Server

PostgreSQL (optional for persistence)

Redis (optional for caching)

Setup

1. Clone the Repository

git clone <repository-url>
cd EventDrivenCQRS

2. Install Dependencies

Ensure you have .NET 8 installed.

dotnet restore

3. Update Configuration

Modify appsettings.json to include the following settings:

"RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest"
},
"PostgreSQL": {
    "ConnectionString": "Host=localhost;Database=EventDrivenCQRS;Username=postgres;Password=password"
},
"MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "EventDrivenCQRS"
},
"Redis": "localhost:6379"

4. Start RabbitMQ Server

Ensure RabbitMQ is running. Install it if not already installed:

brew install rabbitmq # macOS
sudo apt-get install rabbitmq-server # Ubuntu
rabbitmq-server # Start server

Enable RabbitMQ Management Plugin for GUI:

rabbitmq-plugins enable rabbitmq_management

Access GUI at: http://localhost:15672 (default credentials: guest/guest).

5. Run the Application

dotnet run --project src/EventDrivenCQRS.Api

6. API Testing

Use tools like Postman or cURL to test the following endpoints:

Publish Message: POST /api/RabbitMq/publish

Key Classes

RabbitMqService: Manages message publishing and queue configuration.

RetryProcessor: Handles retrying failed messages from DLQ.

RabbitMqInitializer: Configures RabbitMQ queues and DLQs.

Testing the Workflow

Publish messages using the /api/RabbitMq/publish endpoint.

Simulate errors by publishing messages containing "Error" in the payload.

Observe message routing to DLQ and retries from DLQ to the main queue.

Folder Structure

EventDrivenCQRS
├── src
│   ├── EventDrivenCQRS.Api            # API project
│   ├── EventDrivenCQRS.Application    # Application logic
│   ├── EventDrivenCQRS.Infrastructure # Infrastructure (RabbitMQ, DB, etc.)
├── tests                              # Unit and integration tests

