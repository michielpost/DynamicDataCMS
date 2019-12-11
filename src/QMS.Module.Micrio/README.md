# Micrio Module
The Micrio module allows you to submit file uploads to Micrio and retreive a Micrio short id. The short id can be stored in your json document and used to view the uploaded image in Micrio.

## Usage
- Register for a micr.io account
- Obtains a ApiKey and User Id
- Add ApiKey and UserId to appsettings.json

```json
  "MicrioConfig": {
    "ApiKey": "YOUR_API_KEY",
    "UserId": "YOUR_USER_ID",
    "FolderShortId": "folderShortId"
  }
```

- Reference the QMS.Module.Micrio NuGet package
- Add `.ConfigureMicrio()` to the UseQms in Startup.cs
- Changes needed in the schema:
Add a micrioField to the uploader options:
```json
"options": {
        "upload": {
          "upload_handler": "uploadHandler",
          "micrioField": "micrioId"
        }
      },
```
Add the micrioId field to the document:
```json
"micrioId": {
        "type": "string",
        "readonly": true
      }
```