// * Project And Solution
/*
    ❕Create new Project
        dotnet new console/webapi/classlib/mvc -n [ProjectName]

    ❕Create new Solution
        dotnet new sln -n [SolutionName]

    ❕Add Project To Solution
        dotnet sln add [ProjectName]
*/

//  * Run & Build
/*
    ❕Run Project
        dotnet run --project [ProjectName]

    ❕Build Solution
        dotnet build
*/

// * EF Core CLI Notes
/*
    To use Entity Framework Core commands with the CLI, the 
    Microsoft.EntityFrameworkCore.Design package is required.

    This package is installed automatically if you add 
    Microsoft.EntityFrameworkCore.Tools package.

    Add Migration
        dotnet ef migrations add "YourMigrationName"
            --project [Project that contains the DbContext]
            --startup-project [Project that contains Program.cs]
            --output-dir [Directory where migrations will be stored]
            --context [DbContextName]

    Update Database
        dotnet ef database update

    Remove Last Migration
        dotnet ef migrations remove

    List All Migrations
        dotnet ef migrations list
*/

// * NuGet Packages
/*
    ❕Add Package
        dotnet add package [PackageName] --version [Version]
        dotnet add [ProjectName] package [PackageName] --version [Version]

    ❕Remove Package
        dotnet remove package [PackageName]
        dotnet remove [ProjectName] package [PackageName]

*/