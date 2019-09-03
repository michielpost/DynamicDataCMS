# QMS4
Open source Q42 CMS   
Headless CMS based on JsonSchema standard

## Features
- Headless CMS
- JsonSchema used to describe entities
- View and edit entities
- Support for multiple datastores (CosmosDB, Blob Storage)
- Support for entities in multiple languages
- Upload images and other assets
- Generate JsonSchema from C# Models
- Easy installation using NuGet packages

## Installation Instructions
Install `QMS.Core` and `QMS.Storage.CosmosDB` from NuGet

Optional: `QMS.Storage.AzureStorage` 

Edit `Program.cs` and add the following lines to `CreateWebHostBuilder`   

```cs
    .UseQms(new CmsBuilder()
                            .AddAzureStorage()
                            .AddCosmosDB())
```

Add configuration to your appsettings.json
```json
"CosmosConfig": {
    "Endpoint": "https://localhost:8081",
    "Key": "CosmosDB-key"
  },
  "AzureStorageConfig": {
    "StorageAccount": "UseDevelopmentStorage=true",
    "ContainerName": "cms",
    "AssetContainerName": "cms"
  }
```

## Installation Instructions for Development
- Install CosmosDB emulator for Windows https://aka.ms/cosmosdb-emulator
- Edit appsettings.json with Cosmos Endpoint and Key
- Install Azure Storage Emulator
- Run QMS4

## Dependencies
JSON Schema Editor
https://github.com/json-editor/json-editor

Azure Cosmos DB
https://github.com/Azure/azure-cosmos-dotnet-v3

NJsonSchema
https://github.com/RicoSuter/NJsonSchema


## Roadmap
- Searching and ordering on list view

- Pages and url tree

- Multiple versions of items (with start and end time)

- Multiple Data Stores (write to blob storage)

- Configure the CMS from within the CMS

- Website SDK / Website usage example

- Authentication and Authorization

- Audit Trail (CosmosDB Change Feed / Blob Storage file)

- Build in JsonSchema Designer, some ideas:  
https://bjdash.github.io/JSON-Schema-Builder/  
https://jsondraft.com/4c/#tree  
https://mozilla-services.github.io/react-jsonschema-form/  
https://jsoneditoronline.org/  
