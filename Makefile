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
	@echo "  start-server    Start the DynamoDB and API containers."
	@echo "  run-client      Start the client container."
	@echo "  stop-containers Stop and remove all containers."
	@echo "  help            Show this help message."
	@echo ""

start-server:
	docker-compose up -d $(DYNAMO_CONTAINER_NAME)
	sleep 5
	docker-compose up -d $(API_CONTAINER_NAME) --build

run-client:
	docker-compose up -d $(CLIENT_CONTAINER_NAME)

stop-containers:
	docker-compose down

.PHONY: start-server run-client stop-containers help
