# RacksStand
Rack-Based Inventory Management System

## Introduction

This document outlines the requirements and assumptions for a rack-based inventory management system designed to help shopkeepers efficiently manage their stock within racks containing pockets or boxes. The system aims to address the challenges of manually locating products, reducing search time, and preventing lost sales due to misplaced items.

`src>:`

```cmd
dotnet ef migrations add InitialCreate --project  RacksStand.Infrastructure/RacksStand.Infrastructure.csproj --startup-project  RacksStand.Api/RacksStand.Api.csproj

dotnet ef database update --project  RacksStand.Infrastructure/RacksStand.Infrastructure.csproj --startup-project  RacksStand.Api/RacksStand.Api.csproj

```

# Migration creation

```cmd
dotnet --% ef migrations script --idempotent --output ./migrations.sql --project ./src/backend/modules/RacksStands.Module.UserManagement/RacksStands.Module.UserManagement.csproj --startup-project ./src/backend/hosts/RacksStands.ApiHost/RacksStands.ApiHost.csproj
```

## API -
```
https://localhost:7097/openapi/v1.json
```
 