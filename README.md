# Phoesion.Glow.Skoruba.Duende.IdentityServer.Admin

> The administration for the Duende IdentityServer and Asp.Net Core Identity

## Installation via dotnet new template

- Install the dotnet new template:

- 🔒 **NOTE:** The project uses the default database migrations that affect your database, therefore double-check the migrations according to your database provider and create a database backup

```sh
dotnet new install Phoesion.GlowSkoruba.Duende.IdentityServer.Admin.Templates::2.0.0
```

### Create new project:

```sh
dotnet new phoesion.glow.skoruba.duende.isadmin --name MyProject --title MyProject --adminemail "admin@example.com" --adminpassword "Pa$$word123" --adminrole MyRole --adminclientid MyClientId --adminclientsecret MyClientSecret --dockersupport false
```

Project template options:

```
--name: [string value] for project name
--adminpassword: [string value] admin password
--adminemail: [string value] admin email
--title: [string value] for title and footer of the administration in UI
--adminrole: [string value] for name of admin role, that is used to authorize the administration
--adminclientid: [string value] for client name, that is used in the Duende IdentityServer configuration for admin client
--adminclientsecret: [string value] for client secret, that is used in the Duende IdentityServer configuration for admin client
--dockersupport: [boolean value] include docker support
```