# Rui Santos Laboratory

Welcome to the Rui Santos Laboratory, an ASP.NET Core API that streamlines medical appointment scheduling and management for patients and doctors.

## Introduction

This repository contains a comprehensive solution for medical appointment scheduling, providing features for both patients and doctors.

## Features

- Schedule medical appointments with doctors from various specialties.
- View and manage appointments for patients.
- Easily cancel appointments when needed.
- Doctors can view their appointments for the day.

## Technologies Used

This project was built using the following technologies:

- Services:
  - [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
  - [Swagger](https://swagger.io/)
  - [GraphQL](https://graphql.org/)
  - [HotChocolate](https://chillicream.com/docs/hotchocolate/v13)
- Client:
  - [Blazor Webassembly](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
  - [Blazorise](https://blazorise.com/)
  - [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- Tests:
  - [xUnit Tests](https://xunit.net/)
  - [Testcontainers](https://dotnet.testcontainers.org/)
  - [NSubstitute](https://nsubstitute.github.io/)
- Host:
  - [AWS Dynamodb](https://aws.amazon.com/en/pm/dynamodb/)
  - [Docker](https://www.docker.com/)


## Getting Started
To set up the Rui Santos Laboratory on your local machine, follow these steps:

1. Clone this repository:
```bash
git clone https://github.com/ruisantos78/Laboratory.git
```

2. Navigate to the root directory of the project:
```bash
cd Laboratory
```

3. Build and start the API using Docker Compose:
```bash
docker-compose up --build
```

4. The API will now be available at `http://localhost:8001`.

### API Documentation

API documentation is available at `http://localhost:8001/swagger`.

GraphQL documentation is available at `http://localhost:8001/graphql`.

## Makefile Commands

For simplified project management, we provide a Makefile with the following commands:

- make build: Start the DynamoDB and API containers.
- make run: Start the client container.
- make kill: Stop and remove all containers.
- make help: Show this help message.

To use these commands, make sure you have the make utility installed on your system.

## Contributing

Pull requests and bug reports are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the [MIT](https://opensource.org/licenses/MIT) License.

## Acknowledgments

This API was built using [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) and [Docker](https://www.docker.com/). Thanks to their respective communities for their hard work and contributions.

## Author

This API was created by [Rui Sergio Carvalho dos Santos](https://github.com/ruisantos78).
