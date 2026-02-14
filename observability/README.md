##  Observability Flow
```
Application Services
        |
        | OTLP (gRPC) :4317
        v
OpenTelemetry Collector
        |
        |-- traces --------------> Tempo        :3200
        |
        |-- logs ----------------> Loki         :3100
        |
        |-- exposes metrics -----> :8889 (Prometheus metrics endpoint)
                                       ^
                                       |
                                       | scrapes (pull model)
                                 Prometheus :9090 (UI / storage)

Grafana :3000 (reads from Tempo, Loki, Prometheus)

```