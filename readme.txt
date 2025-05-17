
IndependentTrees.API\appsettings.json :

if you want to use  PostgreSQL  -  configure 'PostgreSqlConnection'
if you want to use MsSQL - remove\rename 'PostgreSqlConnection' and configure MsSqlConnection

The database that you specified in the connection string will be created on first start of app


Quick start - dotnet_run.cmd

Swagger:
https://localhost:7117/swagger/index.html