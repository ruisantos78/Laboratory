# ZocDoc Backend Test API

This repository contains an ASP.NET Core API that facilitates medical appointments scheduling and management for patients and doctors.

## Features
- Schedule medical appointments with doctors from different specialties
- View all appointments for a patient
- Cancel appointments as needed
- View all appointments for a doctor for the day

## Technologies Used
This API was built using the following technologies:

- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
- [AWS Dynamodb](https://aws.amazon.com/en/pm/dynamodb/)
- [Swagger](https://swagger.io/)
- [Docker](https://www.docker.com/)
- [Testcontainers](https://dotnet.testcontainers.org/)
- [Autofac](https://autofac.org/)
- [xUnit Tests](https://xunit.net/)

## Getting Started
To set up the ZocDoc Backend Test API on your local machine, follow these steps:

1. Clone this repository:
```
git clone https://github.com/ruisantos78/ZocDoc.git
```

2. Navigate to the root directory of the project:
```
cd ZocDoc
```

3. Build and start the API using Docker Compose:
```
docker-compose up --build
```

4. The API will now be available at `http://localhost:8001`.

### API Documentation

API documentation is available at `http://localhost:8001/swagger`.

GraphQL documentation is available at `http://localhost:8001/graphql`.

## Contributing

Pull requests and bug reports are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the [MIT](https://opensource.org/licenses/MIT) License.

## Acknowledgments

This API was built using [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) and [Docker](https://www.docker.com/). Thanks to their respective communities for their hard work and contributions.

## Author

This API was created by [Rui Sergio Carvalho dos Santos](https://github.com/ruisantos78).
