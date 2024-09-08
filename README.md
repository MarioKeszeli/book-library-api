# Book Library
Application for management of borrowed books

This service has the following responsabilities:

1. CRUD operations for managing library users
2. CRUD operations for managing library books
3. operations for borrowing and returning books
4. automatic sending of an email reminder to the user the day before the due date for returning the book

## Technologies used

-   Technology stack: .NET (Core) 8.0
-   Implementation: Microservice
-   Dependencies: Azure Cosmos DB, Azure Functions

---

## Getting Started

### Prerequisities

- Microsoft Visual Studio 2022 with following components
    - ASP.NET and web development
    - .NET desktop development
    - .NET 8.0 Runtime (Long Term Support)
- CosmosDB with following configuration
    - database with name "BookLibraryDB"
    - and containers "Books" with partition key "/id", "Borrowings" with partition key "/id", "Users" with partition key "/id"
    - database with name "TestDB"
    - and containers "Books" with partition key "/id", "Borrowings" with partition key "/id", "Users" with partition key "/id"
- Database connection string

    Example:

    ```
    {
      "ConnectionStrings": {
        "CosmosDB": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
      }
    }
    ```
- For local development, it is advisable to make sure Azure Cosmos DB Emulator and Azurite are running before launching the Api

### Secrets

To run all the projects locally you'll need to set secrets/connection string in `appsettings` and `local.settings`.

---

## Project structure

```text
 src          [ Source code                                       ]
 ├ common     [ Common modules/libraries shared across services   ]
 ├ functions  [ Azure Functions                                   ]
 ├ services   [ REST API and controllers                          ]
 └ tests      [ Integration tests                                 ]
```