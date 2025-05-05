# Asset Management API  

## Overview  
The Asset Management API is a RESTful service built with ASP.NET Core (.NET 8) that allows users to manage assets and their associated balance entries. It provides endpoints to add assets, retrieve assets as of a specific date, and fetch historical balances for a given asset.  

## Prerequisites  
- .NET 8 SDK  
- A configured database (e.g., SQL Server) compatible with Entity Framework Core  

## Endpoints  

### 1. Add Assets  
**Endpoint:** `POST /api/Assets/AddAssets`  
**Description:** Adds a list of assets to the database.  

**Request Body:**
**Response:**  
- `200 OK` if assets are successfully added.  
- `400 Bad Request` if no assets are provided.  

---

### 2. Get Assets As Of Date  
**Endpoint:** `GET /api/Assets/GetAssetsAsOfDate`  
**Description:** Retrieves assets with their most recent balance as of a specified date.  

**Query Parameters:**  
- `date` (required): The date to filter balances (e.g., `2023-01-01`).  

**Response:**
- `200 OK` with the list of assets and their balances.  

---

### 3. Get Historical Balances  
**Endpoint:** `GET /api/Assets/GetHistoricalBalances`  
**Description:** Retrieves all historical balances for a specific asset, ordered by date in descending order.  

**Query Parameters:**  
- `assetId` (required): The ID of the asset to retrieve balances for.  

**Response:**
- `200 OK` with the asset and its historical balances.  
- `404 Not Found` if the asset with the given ID does not exist.  

---

## Models  

### Asset  
The `Asset` class represents an entity in the system that holds information about a specific asset. Each asset has a unique identifier and other properties that describe it.  

**Properties:**  
- `Id` (int): The unique identifier for the asset.  
- `Name` (string): The name of the asset.  
- `Description` (string): A brief description of the asset.  
- `CreatedDate` (DateTime): The date and time when the asset was created.  
- `BalanceEntries` (ICollection<BalanceEntry>): A collection of balance entries associated with the asset.  

### BalanceEntry  
The `BalanceEntry` class represents a record of a balance for a specific asset at a given point in time.  

**Properties:**  
- `Id` (int): The unique identifier for the balance entry.  
- `AssetId` (int): The foreign key linking the balance entry to an asset.  
- `Balance` (decimal): The balance amount for the asset.  
- `EntryDate` (DateTime): The date and time when the balance was recorded.  

**Relationships:**  
- Each `BalanceEntry` is associated with one `Asset`.  

## Database Context  

### AssetDbContext  
The `AssetDbContext` class is the Entity Framework Core database context for the application. It manages the interaction between the application and the database.  

**Key Features:**  
- DbSet properties for the `Asset` and `BalanceEntry` models:  
  - `DbSet<Asset> Assets`: Represents the `Assets` table in the database.  
  - `DbSet<BalanceEntry> BalanceEntries`: Represents the `BalanceEntries` table in the database.  
- Configures relationships and constraints between the `Asset` and `BalanceEntry` models.  

**Example Configuration:**  
- One-to-many relationship between `Asset` and `BalanceEntry` is defined using the `AssetId` foreign key.  
- Ensures cascading delete behavior so that when an `Asset` is deleted, its associated `BalanceEntries` are also removed.

### AssetDbContext
## Running the Project  
1. Clone the repository.  
2. Configure the database connection string in `appsettings.json` using MS SQL Server's local instance.  
3. Run database script mentioned mentioned in file `Db_script.txt` in MS SQL Server:
4. Start the API:
## Testing the API  
Run this solution from VS Studio 2022 and it will open up Swagger UI on browser using localhost. Call the endpoints to test them. Other tools like Postman or cURL can also be used to test the endpoints.  

## License  
This project is licensed under the MIT License.
