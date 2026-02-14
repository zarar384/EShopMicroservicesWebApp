#!/bin/bash

set -e

# корневая директория проекта (где лежит bootstrap.sh)
PROJECT_ROOT=$(pwd)

# директория с приложением (docker-compose.yml)
SRC_DIR="$PROJECT_ROOT/src"

# директория со стеком observability
OBS_DIR="$PROJECT_ROOT/observability"

echo "* Starting bootstrap process..."

# проверка, установлен ли docker
if ! command -v docker &> /dev/null
then
    echo "Docker is not installed."
    exit 1
fi

# проверка, запущен ли docker daemon
if ! docker info > /dev/null 2>&1
then
    echo "Docker daemon is not running."
    exit 1
fi

echo "* Docker is ready."

# проверка структуры папки observability
if [ ! -d "$OBS_DIR" ]; then
    echo "Observability directory not found."
    exit 1
fi

# список обязательных конфигов
REQUIRED_FILES=(
    "$OBS_DIR/docker-compose.observability.yml"
    "$OBS_DIR/otel/otel-collector-config.yaml"
    "$OBS_DIR/tempo/tempo.yaml"
    "$OBS_DIR/loki/loki-config.yaml"
    "$OBS_DIR/prometheus/prometheus.yml"
)

# проверка наличия всех конфигов
for file in "${REQUIRED_FILES[@]}"; do
    if [ ! -f "$file" ]; then
        echo "Missing required file: $file"
        exit 1
    fi
done

echo "* Observability configuration validated."

# создание директорий для хранения данных (persist)
echo "Ensuring data directories..."

# Loki data directories
mkdir -p "$OBS_DIR/loki/data"
mkdir -p "$OBS_DIR/loki/data/chunks"
mkdir -p "$OBS_DIR/loki/data/rules"

# Tempo data directory
mkdir -p "$OBS_DIR/tempo/data"

# Prometheus data directory
mkdir -p "$OBS_DIR/prometheus/data"

echo "Data directories ensured."

# запуск стека observability (tempo, loki, prometheus, collector)
echo "Starting observability stack..."

cd "$OBS_DIR"
docker compose -f docker-compose.observability.yml up -d --build

echo "Waiting for OpenTelemetry Collector..."
sleep 10

cd "$PROJECT_ROOT"

echo "* Observability stack started."

# запуск основного приложения
echo "* Starting application stack..."

cd "$SRC_DIR"
docker compose up -d --build

cd "$PROJECT_ROOT"

echo "* All systems started successfully!"
echo ""
echo "Grafana:    http://localhost:3000 (admin/admin)"
echo "Prometheus: http://localhost:9090"
echo "Tempo:      http://localhost:3200"
echo "Loki:       http://localhost:3100"
echo ""
