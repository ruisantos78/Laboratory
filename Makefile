# Define container names as variables
AWS_CONTAINER_NAME := ruisantos.labs.localstack
API_CONTAINER_NAME := ruisantos.labs.api
GUI_CONTAINER_NAME := ruisantos.labs.client

# Default target (executed when you type just 'make')
default: help

help:
	@echo ""
	@echo "Usage: make <command>"
	@echo ""
	@echo "Available commands:"
	@echo ""
	@echo "  start			Build and start the all containers."
	@echo "  build-server	Start the DynamoDB and API containers."
	@echo "  build-client   Start the client container."
	@echo "  kill			Stop and remove all containers."
	@echo "  update-client	Update the GraphQL schema on the client container (requires the build-server command to be run first)."
	@echo "  run			Start and watch the client application on local machine."
	@echo "  help			Display this help message."
	@echo ""

start:
	docker compose up -d $(AWS_CONTAINER_NAME)
	docker compose up -d $(API_CONTAINER_NAME) --build
	docker compose up -d $(GUI_CONTAINER_NAME) --build

build-server:	
	docker compose up -d $(AWS_CONTAINER_NAME)
	sleep 5
	docker compose up -d $(API_CONTAINER_NAME) --build

build-client:
	docker compose up -d $(GUI_CONTAINER_NAME) --build

kill:
	docker compose down

update-client:
	dotnet graphql update -p Server/RuiSantos.Labs.Client
	dotnet build

run:
	dotnet watch run --urls http://localhost:8002 --project Server/RuiSantos.Labs.Client

.PHONY: start build-server build-client run kill update-client help
