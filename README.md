
# SchemaGenerator and Azure AD B2C protected .NET Core 3 WebApi

This sample contains a solution file that contains two projects: `SchemaGenerator` and `ExampleDbLib`.

- `SchemaGenerator` is a .NET Core Web Application / API project
- `ExampleDbLib` is a .Net Standard 2.0 project

The following features are demonstrated:
* EntityFrameWork Core is used for object/relational mapping. 
* EF Core code first approach generates the database creation scripts for SQL Server database.
* The backend uses jwt token authentication to protect the delete endpoints. Other endpoints (get, put, post) are public for testing purposes.
* The backend uses reflection and attributes to generate a schema describing the object properties.
* The schema is passed to the frontend, which uses it to generate the UI
* The schema also contains validation info (Required, Range, StringLength, DataType, ReadOnly)
        