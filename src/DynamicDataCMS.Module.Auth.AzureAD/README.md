# Azure AD Authentication
The Azure AD Authentication module allows you to sign in using Azure AD. The User must also be present in the CMS user list.

## Usage
- Uses Microsoft.Identity.Web package
- https://github.com/AzureAD/microsoft-identity-web/wiki/web-apps

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "msidentitysamplestesting.onmicrosoft.com",
    "TenantId": "7f58f645-c190-4ce5-9de4-e2b7acd2a6ab",
    "ClientId": "86699d80-dd21-476a-bcd1-7c1a3d471f75",
    "ClientSecret": "[Copy the client secret added to the app from the Azure portal]"
  },
...
}
```

- Reference the DynamicDataCMS.Module.Micrio NuGet package
- Add `.ConfigureMicrio()` to the UseDynamicDataCMS in Startup.cs
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
