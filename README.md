# QMS4
Open source Q42 CMS   
Headless CMS based on JsonSchema standard

## Features
- Headless CMS
- JsonSchema used to describe entities
- View and edit entities
- Support for multiple datastores (CosmosDB, Blob Storage)
- Support for entities in multiple languages
- Easy installation using NuGet packages

## Installation Instructions
- Install CosmosDB emulator for Windows https://aka.ms/cosmosdb-emulator
- Edit appsettings.json with Cosmos Endpoint and Key
- Run QMS4

## Dependencies
JSON Schema Editor
https://github.com/json-editor/json-editor

Azure Cosmos DB
https://github.com/Azure/azure-cosmos-dotnet-v3

NJsonSchema
https://github.com/RicoSuter/NJsonSchema


## Roadmap
- Asset Manager for Images and other files

- Searching and ordering on list view

- Pages and url tree

- Multiple Data Stores (write to blob storage)

- Website SDK / Website usage example

- Authentication and Authorization

- Audit Trail

- Build in JsonSchema Designer, some ideas:  
https://bjdash.github.io/JSON-Schema-Builder/  
https://jsondraft.com/4c/#tree  
https://mozilla-services.github.io/react-jsonschema-form/  
https://jsoneditoronline.org/  
