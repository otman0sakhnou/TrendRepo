# TrendScope Worker Service

This application is a background service written in C# that fetches trend data from SerpAPI, processes it, and stores the results in a MongoDB database.

## Features

- Periodically fetches trend data for specified queries from SerpAPI.
- Processes the JSON response to extract and aggregate trend data.
- Stores processed data in a MongoDB collection.
- Includes logging for success, warnings, and errors.

---

## Prerequisites

1. **.NET Core SDK**

   - Version: >= 6.0

2. **MongoDB**

   - Local or remote MongoDB server must be running.

3. **API Key**

   - Obtain an API key from [SerpAPI](https://serpapi.com/).

---

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd TrendScope
```

### 2. Configure MongoDB Connection

In the `Worker` class constructor, update the MongoDB connection string:

```csharp
var mongoClient = new MongoClient("mongodb://<your-mongo-host>:<port>");
var database = mongoClient.GetDatabase("TrendScopeDB");
```

### 3. Configure API Key and Query

In the `ExecuteAsync` method, replace the `apiKey` and `query` with your desired values:

```csharp
string apiKey = "<your-serpapi-key>";
string query = "artificial intelligence,machine learning,deep learning";
```

### 4. Run the Application

Execute the application using the .NET CLI:

```bash
dotnet run
```

---

## Project Structure

```plaintext
TrendScope/
|-- Program.cs           // Entry point for the application
|-- Worker.cs            // Background service logic
|-- appsettings.json     // Configuration file (optional for logging or MongoDB settings)
```

---

## Workflow

1. **Fetch Trends:**

   - The worker periodically fetches data from the SerpAPI endpoint.

2. **Process JSON:**

   - The JSON response is parsed using `Newtonsoft.Json.Linq`.
   - The `interest_over_time` data is extracted and aggregated.

3. **Store in MongoDB:**

   - The aggregated data is saved as documents in the `Trends` collection in MongoDB.

---

## Dependencies

- [MongoDB.Driver](https://www.nuget.org/packages/MongoDB.Driver): Interacts with MongoDB.
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json): Parses JSON data.
- [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting): Provides support for background services.

Install dependencies using NuGet:

```bash
dotnet add package MongoDB.Driver
```

```bash
dotnet add package Newtonsoft.Json
```

---

## Logging

Logs are implemented using the built-in .NET `ILogger` interface. Examples:

- **Success:** "Data successfully processed and stored in MongoDB."
- **Warnings:** "No trends found for the query."
- **Errors:** Detailed exception messages during failures.

---

---

## Author

Sakhnou Otmann
