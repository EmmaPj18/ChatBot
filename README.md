# ChatBot

## Description

This is a simple chatbot that use a simple command based solution to get stock data from this api (https://stooq.com/q/l/).

## Features

- Authentication using IdentityServer
- Chatroom using SignalR on a Blazor Server App
- Implementation of the command /stock=stock_code to get stock data from the api (https://stooq.com/q/l/)
- Message ordered by timestamp on descending order
- Only get lastest 50 messages on the chatroom
- Unit tests of the BotCommandService

### Bonus tasks completed

- Use of .NET identity to authenticate users
- Handle messages that are not undestood or any exceptions raised within the bot
- Build of an installer using Docker and Docker Compose

## Build with

- Docker
- .NET 7
- Blazor Server
- SignalR
- RabbitMQ
- Sql Server 2022
- CsvHelper
- IdentityServer

## Prerequisites

- Docker
- Docker Compose

or 

- .NET 7 Runtime
- Microsoft SQL Server
- RabbitMQ

## Installation

### Docker

- On Root folder run the command `docker-compose up -d --build`
- Update connection string on the appsettings.json file on the ChatBot.Data project folder
- Go to the ChatBot.Data project folder and run the command `dotnet ef database update` to run migrations
- Then access the url http://localhost:4000
- Create an account and login
- Go to the chatroom

### Without Docker

- Start the RabbitMQ Server And Microsoft SQL Server
- Update the connection string on the appsettings.json file on the ChatBot.Bot project, the ChatBot.App project and the ChatBot.Data project
- Go to the ChatBot.Data project folder and run the command `dotnet ef database update` to run migrations
- Go to the ChatBot.Bot project folder and run the command `dotnet run`
- Go to the ChatBot.App project folder and run the command `dotnet run`
- o to the url http://localhost:7158
- Create an account and login
- Go to the chatroom

