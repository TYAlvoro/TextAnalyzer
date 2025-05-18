# TextAnalyzer

**TextAnalyzer** — микросервисное приложение на C#, реализующее загрузку, хранение и анализ текстовых файлов.

---

## Содержание

* [Описание проекта](#описание-проекта)
* [Архитектура](#архитектура)
* [Структура решения](#структура-решения)
* [Требования](#требования)
* [Установка и запуск](#установка-и-запуск)

    * [Через Docker Compose](#через-docker-compose)
    * [Локальная разработка](#локальная-разработка)
* [API Endpoints](#api-endpoints)
* [Тестирование](#тестирование)

    * [Swagger UI](#swagger-ui)
    * [curl](#curl)
    * [Unit-тесты](#unit-тесты)
* [Примеры текстовых файлов](#примеры-текстовых-файлов)
* [Troubleshooting](#troubleshooting)

---

## Описание проекта

Система состоит из трёх микросервисов:

1. **FileStoreService** — хранит и выдаёт загруженные `.txt` файлы.
2. **FileAnalysisService** — анализирует текст: подсчитывает абзацы, слова, символы.
3. **ApiGateway** — маршрутизирует запросы клиента к внутренним сервисам.

Антиплагиат (сравнение файлов) не реализован по варианту 1.

---

## Архитектура

```plaintext
Client ↔ ApiGateway ↔ { FileStoreService, FileAnalysisService }
```

* Клиент посылает запросы к ApiGateway
* ApiGateway пересылает их в FileStore или FileAnalysis
* FileStore хранит файлы в Docker volume `filestore_data`
* FileAnalysis читает файлы из общего volume и возвращает результаты

---

## Структура решения

```
TextAnalyzer/
├── ApiGateway/
│   └── ...          # ASP.NET Core Web API (порт 5000)
├── FileStoreService/
│   └── ...          # ASP.NET Core Web API (порт 8080)
├── FileAnalysisService/
│   └── ...          # ASP.NET Core Web API (порт 8081)
├── Shared/          # Общие DTO: FileUploadResponse, AnalysisRequest, AnalysisResult
├── docker-compose.yml
├── README.md
└── TextAnalyzer.sln
```

---

## Требования

* .NET 8 SDK
* Docker & Docker Compose

---

## Установка и запуск

### Через Docker Compose

1. Клонируйте репозиторий и перейдите в корень решения:

   ```bash
   git clone https://github.com/TYAlvoro/TextAnalyzer
   cd TextAnalyzer
   ```
2. Запустите контейнеры:

   ```bash
   docker-compose up --build
   ```
3. Дождитесь логов вида `Now listening on: http://[::]:80` для каждого сервиса.
4. **Swagger UI**:

    * Gateway: [http://localhost:5000/swagger](http://localhost:5000/swagger)
    * FileStore: [http://localhost:8080/swagger](http://localhost:8080/swagger)
    * FileAnalysis: [http://localhost:8081/swagger](http://localhost:8081/swagger)

### Локальное тестирование

1. Откройте `TextAnalyzer.sln` в Rider.
2. Установите порты в `launchSettings.json`:

    * FileStoreService: 8080
    * FileAnalysisService: 8081
    * ApiGateway: 5000
3. Запустите конфигурацию Multi-Start (все три проекта).

---

## API Endpoints

| Метод                              | Описание                              |
| ---------------------------------- | ------------------------------------- |
| **POST** `/api/gateway/files`      | Загрузка `.txt` файла                 |
| **GET**  `/api/gateway/files/{id}` | Скачивание загруженного файла по ID   |
| **POST** `/api/gateway/analysis`   | Анализ файла (абзацы, слова, символы) |

Примеры запросов см. раздел [curl](#curl).

---

## Тестирование

### Swagger UI

1. Перейдите в Swagger UI Gateway: [http://localhost:5000/swagger](http://localhost:5000/swagger)
2. Загрузите файл (POST `/api/gateway/files`)
3. Скопируйте `fileId` и запросите анализ (POST `/api/gateway/analysis`)
4. Скачайте файл (GET `/api/gateway/files/{id}`)

### curl

```bash
# 1. Загрузка
curl -X POST http://localhost:5000/api/gateway/files \
     -F file=@./test-files/sample1.txt

# 2. Анализ
curl -X POST http://localhost:5000/api/gateway/analysis \
     -H "Content-Type: application/json" \
     -d '{"fileId":"<fileId>"}'

# 3. Скачивание
curl http://localhost:5000/api/gateway/files/<fileId> \
     --output downloaded.txt
```

### Unit-тесты

Запустите все тесты:

```bash
dotnet test FileStoreService.Tests/FileStoreService.Tests.csproj
dotnet test FileAnalysisService.Tests/FileAnalysisService.Tests.csproj
dotnet test ApiGateway.Tests/ApiGateway.Tests.csproj
```

---

## Примеры текстовых файлов

Уже создана папка `test-files` рядом с решением:

* `sample1.txt`:

  ```text
  Hello world!
  This is a test file.

  It has three paragraphs and a total of fourteen words.
  ```

* `sample2.txt`:

  ```text
  Lorem ipsum dolor sit amet, consectetur adipiscing elit.
  Vestibulum at nunc ac nisi vehicula ultrices.

  Donec fermentum, nisl a bibendum cursus, magna leo venenatis massa, et scelerisque justo tellus at orci.
  ```
---

Готово! Этот README содержит всё необходимое для быстрой сборки, запуска и проверки проекта.
