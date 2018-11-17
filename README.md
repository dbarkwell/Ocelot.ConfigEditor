# Ocelot.ConfigEditor
A configuration editor for Ocelot (https://github.com/TomPallister/Ocelot)

## Authorization
There are two ways to authorize access to the config editor. The authorization types are:

1. Available only on localhost (default)
2. Authenticating against a third party identity service. Currently, AzureAD, Google, and Open Id Connect are configured.
  
### Enable Azure AD Authentication
1. Sign into Azure Portal
1. Click Azure Active Directory
1. Click App registrations (Preview)
1. Click New registration
1. Add Name and change Supported account types if required. Add Redirect URI.

### Enable Google Authentication

TBD

### Enable Open Id Connect Authentication

#### Configure with Azure Active Directory

1. Set up a new Azure Active Directory (AAD) in your Azure Subscription.
1. Open the newly created AAD in Azure web portal.
1. Navigate to the Applications tab.
1. Add a new Application to the AAD. Set the "Sign-on URL" to sample application's URL.
1. Navigate to the Application, and click the Configure tab.
1. Find and save the "Client Id".
1. Add a new key in the "Keys" section. Save value of the key, which is the "Client Secret".
1. Click the "View Endpoints" on the drawer, a dialog will shows six endpoint URLs. Copy the "OAuth 2.0 Authorization Endpoint" to a text editor and remove the "/oauth2/authorize" from the string. The remaining part is the authority URL. It looks like https://login.microsoftonline.com/<guid>.

#### Configure with Google Identity Platform

1. Create a new project through Google APIs.
1. In the sidebar choose "Credentials".
1. Navigate to "OAuth consent screen" tab, fill in the project name and save.
1. Navigate to "Credentials" tab. Click "Create credentials". Choose "OAuth client ID".
1. Select "Web application" as the application type. Fill in the "Authorized redirect URIs" with https://localhost:44318/signin-oidc.
1. Save the "Client ID" and "Client Secret" shown in the dialog.
1. The "Authority URL" for Google Authentication is https://accounts.google.com/.

