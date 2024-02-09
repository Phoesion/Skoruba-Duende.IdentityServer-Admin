param([string] $packagesVersions)

$templateNuspecPath = "template-publish/Phoesion.Glow.Skoruba.Duende.IdentityServer.Admin.Templates.nuspec"
nuget pack $templateNuspecPath -NoDefaultExcludes

dotnet new --uninstall Phoesion.Glow.Skoruba.Duende.IdentityServer.Admin.Templates

$templateLocalName = "Phoesion.Glow.Skoruba.Duende.IdentityServer.Admin.Templates.$packagesVersions.nupkg"
dotnet new -i $templateLocalName

dotnet new phoesion.glow.skoruba.duende.isadmin --name MyProject --title MyProject --adminemail 'admin@skoruba.com' --adminpassword 'Pa$$word123' --adminrole MyRole --adminclientid MyClientId --adminclientsecret MyClientSecret --dockersupport true