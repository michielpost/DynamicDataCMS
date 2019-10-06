![GitHub Actions status | Q42/QMS4](https://github.com/Q42/QMS4/workflows/ASP.NET%20Core%20CI/badge.svg)
# QMS4
Open source Q42 CMS   
Headless CMS based on JsonSchema standard

## Features
- Headless CMS
- Support for multiple datastore plugins (CosmosDB, Blob Storage)
- JsonSchema used to describe entities
- View and edit entities
- Paging and ordering in list view
- Support for entities in multiple languages
- Upload images and other assets
- Generate JsonSchema from C# Models
- Easy installation using NuGet packages

## Installation Instructions
Install `QMS.Core` and `QMS.Storage.CosmosDB` from NuGet

Optional: `QMS.Storage.AzureStorage` 

Edit `Startup.cs` and add the following lines to `ConfigureServices`   

```cs
services.UseQms(Configuration)
  .ConfigureCosmosDB(() => new StorageConfiguration() { ReadCmsItems = true })
  .ConfigureAzureStorage(() => new StorageConfiguration() { ReadFiles = true });
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
- Install Azure Storage Emulator https://docs.microsoft.com/nl-nl/azure/storage/common/storage-use-emulator
- Optional (not needed when using emulators): Edit appsettings.json with Cosmos Endpoint and Key
- Run QMS4
- Navigate to https://localhost:44341/cms

## Dependencies
JSON Schema Editor
https://github.com/json-editor/json-editor

Azure Cosmos DB
https://github.com/Azure/azure-cosmos-dotnet-v3

NJsonSchema
https://github.com/RicoSuter/NJsonSchema

JavaScript Notifications
https://ned.im/noty/


## Roadmap
- Searching in list view

- Pages and url tree

- Multiple versions of items (with start and end time)

- Website SDK / Website usage example

- Authentication and Authorization

- Audit Trail (CosmosDB Change Feed / Blob Storage file)

## Ideas

- Configure the CMS from within the CMS

- Build a JsonSchema editor in Blazor?

- Build in JsonSchema Designer, some ideas:  
https://bjdash.github.io/JSON-Schema-Builder/  
https://jsondraft.com/4c/#tree  
https://mozilla-services.github.io/react-jsonschema-form/  
https://jsoneditoronline.org/  
