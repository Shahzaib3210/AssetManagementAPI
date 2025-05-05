# Asset Management API  

## Overview  
The Asset Management API is a .NET 8-based web API for managing assets, balances, and holdings. It provides endpoints for adding assets, retrieving assets as of a specific date, and fetching historical balances for a given asset.  

## Features  
- **Add Assets**: Import multiple assets, including their balances, holdings, and associated metadata.  
- **Retrieve Assets by Date**: Fetch assets with their latest balances as of a specific date.  
- **Historical Balances**: Retrieve historical balance data for a specific asset.  

## Prerequisites  
- .NET 8 SDK  
- Microsoft SQL Server
- Entity Framework Core  

## Installation  
1. Clone the repository:
2. Restore dependencies:
3. Update the `appsettings.json` file with your database connection string.  

4. Apply database migrations:
5. Run the application:
## Endpoints  

### 1. Add Assets  
**POST** `/api/Assets/AddAssets`  
- **Description**: Adds multiple assets, including their balances and holdings.  
- **Request Body**: JSON array of asset objects.  
- **Response**:  
  - `200 OK`: Number of assets imported successfully.  
  - `400 Bad Request`: Validation errors or missing fields.  
  - `500 Internal Server Error`: Database or server errors.  

### 2. Get Assets As Of Date  
**GET** `/api/Assets/GetAssetsAsOfDate`  
- **Description**: Retrieves assets with their latest balances as of a specific date.  
- **Query Parameter**:  
  - `date` (required): The date to filter balances.  
- **Response**:  
  - `200 OK`: List of assets with their latest balances.  
  - `500 Internal Server Error`: Server errors.  

### 3. Get Historical Balances  
**GET** `/api/Assets/GetHistoricalBalances`  
- **Description**: Fetches historical balance data for a specific asset.  
- **Query Parameter**:  
  - `assetId` (required): The ID of the asset.  
- **Response**:  
  - `200 OK`: Historical balances for the asset.  
  - `404 Not Found`: Asset not found.  
  - `500 Internal Server Error`: Server errors.  

## Database Models  

### Asset  
- **Fields**:  
  - `AssetId` (string)  
  - `Nickname` (string)  
  - `WealthAssetType` (string)  
  - `CreationDate` (DateTime)  
  - `Balances` (List of `BalanceEntry`)  
  - `Holdings` (List of `Holding`)  

### AssetInfo  
- **Fields**:  
  - `Nickname` (string)  
  - `EstimateValue` (decimal)  
  - `PurchaseCost` (decimal)  
  - `PurchaseDate` (DateTime?)  

### BalanceEntry  
- **Fields**:  
  - `BalanceAsOf` (DateTime)  
  - `Amount` (decimal)  
  - `BalanceCostBasis` (decimal?)  

### Holding  
- **Fields**:  
  - `MajorClass` (string)  
  - `MinorClass` (string)  
  - `Value` (decimal)  

## Error Handling  
- **400 Bad Request**: For invalid input or missing required fields.  
- **404 Not Found**: For resources that do not exist.  
- **500 Internal Server Error**: For unexpected server or database errors.  

## Technologies Used  
- **Framework**: ASP.NET Core 8  
- **Database**: Entity Framework Core with SQL Server  
- **Serialization**: System.Text.Json  


## Running the Project  
1. Clone the repository.  
2. Configure the database connection string in `appsettings.json` using MS SQL Server's local instance.  
3. Run database script mentioned mentioned in file `Db_script.txt` in MS SQL Server:
4. Start the API:
## Testing the API  
Run this solution from VS Studio 2022 and it will open up Swagger UI on browser using localhost. Call the endpoints to test them. Other tools like Postman or cURL can also be used to test the endpoints.  

## License  
This project is licensed under the MIT License.
