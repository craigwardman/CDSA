> :warning: This code is very old now and I don't see it being useful anymore, so I'm archiving this repo.

# CDSA
"CDSA" is a set of CodeSmith templates and associated infrastructure libraries for a "top to bottom" tiered application architecture. It is here for posterity for building/updating dependant projects, as for the most part the preference for this style of application architecture has been dimished. However, this can still be useful for:

A) Quick to build CRUD applications that are data-first and simply require an abstraction between business logic and data access logic  
B) Used within a repository of a larger software architecture, to act as gateway to the SQL data store (e.g. alternative to EnitityFramework)

Given a compatible SQL database and a copy of CodeSmith, the templates will generate a C# architecture which can then be coded against in Visual Studio. Each output project will contain a folder starting with "_" which is where the generated code should stay. Any modifications you wish to make for your application should be outside of these folders, within the relevent project.

To enable this, all files are marked as partial. You can use inheritance to override the default behaviour or abstraction/interfaces to add functionality that does not exist in the template.

 - BLL.csproj (business logic layer)
  - There will be a manager class for each table in the database   
  - Write your business logic in here

- DDL.csproj (data definition layer)
  - There will be a DTO for each table in the database
  - There is also the abstract schema for the database
  - A good place to define any other DTO/definition classes
  
- DML.csproj (data management layer)
  - An abstraction of data management functions provided by the DAL   
  - Defines the data manager and provider
  - Nothing much changes in here unless you want to extend the interface for the DAL

- SQLDAL.csproj (sql data access layer)
  - A SQL implementation of the data access layer, usually the only one you need
  - Implements the DML and exchanges DTOs

- Interface (Example consumers [ui/service])
 - ServerControls are for ASP.NET WebForms and rely on "ControlExtensions" library
 - RiaServices are for Silverlight RIA service apps

The CodeSmith templates should be run by exectuting "_nTier_.cst" this will execute the other templates as required.

The projects rely on the provided libraries "ClauseWrappers" which is an abstraction on definining predicates. And "SqlHelper" which transforms the abstract clauses to SQL clauses and executes them using ADO.NET.

NOTE on DB compatibility: All tables must use an identity column which must always be of the same type (e.g. int or guid)

For a combined example of extending CDSA and exposing a manager for spatial querying, see:
http://www.craigwardman.com/Blogging/BlogEntry/spatial-querying-example-in-cdsa
