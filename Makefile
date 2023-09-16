# Define container names as variables
DYNAMO_CONTAINER_NAME := ruisantos.labs.db.dynamo
API_CONTAINER_NAME := ruisantos.labs.api
CLIENT_CONTAINER_NAME := ruisantos.labs.client

# Default target (executed when you type just 'make')
default: help

help:
	@echo ""
	@echo "Usage: make <command>"
	@echo ""
	@echo "Available commands:"
	@echo ""
	@echo "  build-server	Start the DynamoDB and API containers."
	@echo "  build-client   Start the client container."
	@echo "  kill			Stop and remove all containers."
	@echo "  update-client	Update the GraphQL schema on the client container (requires the build-server command to be run first)."
	@echo "  help			Display this help message."
	@echo ""

build-server:	
	docker-compose up -d $(DYNAMO_CONTAINER_NAME)
	sleep 5
	docker-compose up -d $(API_CONTAINER_NAME) --build

build-client:
	docker-compose up -d $(CLIENT_CONTAINER_NAME) --build

kill:
	docker-compose down

update-client:
	dotnet graphql update -p Server/RuiSantos.Labs.Client
	dotnet build

.PHONY: build run kill update-client help
