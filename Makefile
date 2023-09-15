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
	@echo "  build    	Start the DynamoDB and API containers."
	@echo "  run      	Start the client container."
	@echo "  kill		Stop and remove all containers."
	@echo "  help		Show this help message."
	@echo ""

build:	
	docker-compose up -d $(DYNAMO_CONTAINER_NAME)
	sleep 5
	docker-compose up -d $(API_CONTAINER_NAME) --build

	dotnet graphql update -p Server/RuiSantos.Labs.Client
	dotnet build

run:
	docker-compose up -d $(CLIENT_CONTAINER_NAME)

kill:
	docker-compose down
	
.PHONY: build run kill help
