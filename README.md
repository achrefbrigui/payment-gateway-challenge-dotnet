# Payment Gateway Challenge

A .NET payment gateway implementation for cko recruitment challenge.

## Architecture

```text
                    ┌─────────────────────┐
                    │  Payment Controller │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │  FluentValidation   │
                    │ (Request Validation)│
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │   Payment Service   │
                    └──────────┬──────────┘
                               │
                  ┌────────────┴────────────┐
                  │                         │
                  ▼                         ▼
        ┌──────────────────┐       ┌──────────────────┐
        │Payment Repository│       │   Bank Client    │
        └──────────────────┘       └────────┬─────────┘
                                            │
                                            ▼
                                   ┌──────────────────┐
                                   │  Bank Simulator  │
                                   └──────────────────┘
```

## CI Workflow

                    ┌─────────────────────┐
                    │ Pull Request → main │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │   GitHub Actions    │
                    └──────────┬──────────┘
                               │
                  ┌────────────┼────────────┐
                  │            │            │
                  ▼            ▼            ▼
              Restore        Build        Docker
                                             │
                                             ▼
                                      Run integration
                                          tests

## Running locally

### Prerequisites

* .NET SDK (for dotnet test command)
* Docker
* Docker Compose
* Datadog API key

### Datadog configuration

Create a `.env` file at the root of the project:

```env
DD_API_KEY=your_datadog_api_key
```

The `.env` file is used by Docker Compose to configure the Datadog Agent.

### Start the application

```bash
docker compose up --build
```

This starts the application, the bank simulator and a Datadog Agent.

## Endpoints

### Payment Gateway

* [Swagger UI](https://localhost:7092/swagger/index.html) — API documentation and testing
* `http://localhost:5067` — HTTP API
* `https://localhost:7092` — HTTPS API

### Bank Simulator

* `http://localhost:8080` — Bank simulator
* `http://localhost:2525` — Mountebank UI

# Release Notes

## v2.0.0

### Breaking Changes

* Updated the DTO types and add JSON contracts

### Features

* Added payment creation and retrieval.
* Added payment domain model & repository abstraction.
* Added FluentValidation for payment requests.
* Added bank simulator client integration.
* Added global exception handling.
* Added logging and observability.

### Tests

* Added domain validation unit tests.
* Added payment creation integration tests.
* Added payment retrieval integration tests.
* Added bank simulator integration tests.

### Infrastructure

* Added Docker Compose support for the payment gateway.
* Added Datadog Agent integration.
* Added GitHub Actions CI for build and test execution.
