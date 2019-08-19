# Ocelot.ConfigEditor
A configuration editor for Ocelot (https://github.com/TomPallister/Ocelot).

[![Build Status](https://dev.azure.com/pelism/Ocelot.ConfigEditor/_apis/build/status/Ocelot.ConfigEditor-ASP.NET%20Core-CI?branchName=master)](https://dev.azure.com/pelism/Ocelot.ConfigEditor/_build/latest?definitionId=12?branchName=master)


## How To

1. Add the Ocelot.ConfigEditor nuget package to an Ocelot application
1. Add the Ocelot.ConfigEditor service with: AddOcelotConfigEditor

Examples:

```
.ConfigureServices(s =>
{
    s.AddOcelot();
    s.AddOcelotConfigEditor();
})
```
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddOcelot();
    services.AddOcelotConfigEditor();
}
```

3. Add the Ocelot.ConfigEditor middleware with: UseOcelotConfigEditor

Examples:

```
.Configure(app => 
{
    app.UseOcelotConfigEditor();
    app.UseOcelot().Wait();
}
```
```
public void Configure(IApplicationBuilder app)
{
    app.UseOcelotConfigEditor();
    app.UseOcelot().Wait();
}
```

4. Build the project
5. The default route is cfgedt. This can be changed by passing in ConfigEditorOptions

Example:

```
app.UseOcelotConfigEditor(new ConfigEditorOptions { Path = "edit" }); 
```

6. The default authorization is localhost. See below for different authorization types.


## Authorization
There are two ways to authorize access to the config editor. The authorization types are:

1. Localhost (default). The configuration page is only accessible from localhost.
1. Authenticating against a third party identity service. Currently, AzureAD, Google, and Open Id Connect are configured.
  
### Enable Azure AD Authentication
1. Sign into Azure Portal
1. Click Azure Active Directory
1. Click App registrations (Preview)
1. Click New registration
1. Add Name and change Supported account types if required. Add Redirect URI

### Enable Google Authentication

1. Create a new project through Google APIs
1. In the Library page page, find Google+ API
1. Click create credentials
1. Choose, Google+ API, Web server, and User data
1. Click "What credentials do I need?"
1. Create an OAuth 2.0 client ID
1. Enter Authorized redirect URIs which is https://{url}:{port}/signin-google
1. Click Create client ID and set up the OAuth 2.0 consent screen
1. Click continue
1. Click Download on Download credentials to download Client and Secret Id


### Enable Open Id Connect Authentication

#### Configure with Azure Active Directory

1. Set up a new Azure Active Directory (AAD) in your Azure Subscription
1. Open the newly created AAD in Azure web portal
1. Navigate to the Applications tab
1. Add a new Application to the AAD. Set the "Sign-on URL" to sample application's URL
1. Navigate to the Application, and click the Configure tab
1. Find and save the "Client Id"
1. Add a new key in the "Keys" section. Save value of the key, which is the "Client Secret"
1. Click the "View Endpoints" on the drawer, a dialog will shows six endpoint URLs. Copy the "OAuth 2.0 Authorization Endpoint" to a text editor and remove the "/oauth2/authorize" from the string. The remaining part is the authority URL. It looks like https://login.microsoftonline.com/<guid>

#### Configure with Google Identity Platform

1. Create a new project through Google APIs
1. In the sidebar choose "Credentials"
1. Navigate to "OAuth consent screen" tab, fill in the project name and save
1. Navigate to "Credentials" tab. Click "Create credentials". Choose "OAuth client ID"
1. Select "Web application" as the application type. Fill in the "Authorized redirect URIs" with https://{url}:{port}/signin-oidc
1. Save the "Client ID" and "Client Secret" shown in the dialog
1. The "Authority URL" for Google Authentication is https://accounts.google.com/

