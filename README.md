# EShop Microservices

Educational project to improve skills in microservice architectures.

Tech focus: ASP.NET Web API, Docker, RabbitMQ, MassTransit, gRPC, YARP
API Gateway, PostgreSQL, Redis, SQLite, SQL Server, Marten, Entity
Framework Core, CQRS, MediatR, DDD, Vertical Architecture, Clean
Architecture, .NET 8, cloud-native environments, Carter,
FluentValidation, Mapster.

------------------------------------------------------------------------

# Architecture

Microservices-based backend with centralized OpenTelemetry-based
observability.

``` text
Application Services

Shopping.Web
        |
        v
Gateway
        |
        v
Basket.API  --gRPC-->  Discount.Grpc
        |
        +--RabbitMQ--> Ordering.API

Catalog.API (independent service)
```

All services export telemetry via OTLP to OpenTelemetry Collector.
Collector routes data to Tempo, Loki and Prometheus. Grafana visualizes.

## Communication 
| From       | To            | Protocol | Type         |
| ---------- | ------------- | -------- | ------------ |
| Web        | Gateway       | HTTP     | Synchronous  |
| Gateway    | APIs          | HTTP     | Synchronous  |
| Basket.API | Discount.Grpc | gRPC     | Synchronous  |
| Basket.API | Ordering.API  | RabbitMQ | Asynchronous |

## Endpoints
| Service       | URL                                            |
| ------------- | ---------------------------------------------- |
| Gateway       | [http://localhost:5000](http://localhost:5000) |
| Basket.API    | [http://localhost:5001](http://localhost:5001) |
| Catalog.API   | [http://localhost:5002](http://localhost:5002) |
| Ordering.API  | [http://localhost:5003](http://localhost:5003) |
| Discount.Grpc | [http://localhost:5004](http://localhost:5004) |


------------------------------------------------------------------------

# Services

### Basket.API

-   ASP.NET Core Web API\
-   Carter\
-   MediatR\
-   FluentValidation\
-   Marten (PostgreSQL)\
-   Redis\
-   gRPC client\
-   MassTransit (RabbitMQ)\
-   HealthChecks

### Ordering.API

-   ASP.NET Core Web API\
-   Carter\
-   MediatR\
-   EF Core\
-   SQL Server\
-   MassTransit\
-   HealthChecks

### Catalog.API

-   ASP.NET Core Web API\
-   Carter\
-   MediatR\
-   Marten (PostgreSQL)\
-   HealthChecks

### Discount.Grpc

-   ASP.NET Core gRPC\
-   EF Core\
-   PostgreSQL

### Gateway (YARP)

-   ASP.NET Core\
-   YARP Reverse Proxy

### Shopping.Web

-   ASP.NET Core MVC / Razor\
-   HttpClient -> Gateway

------------------------------------------------------------------------

# Observability

Centralized via OpenTelemetry.

All services export telemetry using:

OTLP (gRPC)\
Otlp:Endpoint = http://otel-collector:4317

## Tracing

-   ASP.NET Core instrumentation\
-   HttpClient instrumentation\
-   MassTransit activity source\
-   Database instrumentation (Npgsql / SqlClient)\
-   Redis instrumentation

## Metrics

-   ASP.NET Core\
-   HttpClient\
-   Runtime metrics\
-   Prometheus scrape via Collector (:8889)

## Logging

-   OpenTelemetry logging provider\
-   Exported via OTLP to Collector\
-   Routed to Loki

## Stack

Located in:

/observability

Includes:

-   OpenTelemetry Collector\
-   Tempo\
-   Loki\
-   Prometheus\
-   Grafana

## Endpoints

| Component  | URL                    |
|------------|------------------------|
| Grafana    | http://localhost:3000  |
| Prometheus | http://localhost:9090  |
| Tempo      | http://localhost:3200  |
| Loki       | http://localhost:3100  |

> Ports may vary depending on docker-compose configuration.

Grafana default credentials: 
admin / admin

------------------------------------------------------------------------

# Running

From project root:

./bootstrap.sh
