// * Intro
/*
    🗒️What is Entity Framework Core (EF Core)
        Is an Object-Relational Mapper (ORM) for .NET that allows developers to work with databases using C# objects instead of writing SQL queries manually.

    🗒️Key Features of EF Core:
        Cross-platform –> Works on Windows, macOS, and Linux.

        Supports both
            Code-First Approach –> Define your database structure using C# classes (models).
            Database-First Approach –> Generate C# classes from an existing database.

        Migrations –> Manage schema changes without losing data.

        LINQ Queries –> Use LINQ to query databases instead of raw SQL.

        Change Tracking –> Detects and tracks changes in entities for efficient updates.

        Lazy & Eager Loading –> Controls how related data is loaded to optimize performance.

    🗒️Entity Framework Core (EF Core) Inheritance Mapping Strategies
        Define how object-oriented class hierarchies are represented in relational databases.

        [1] Table Per Hierarchy (TPH)
            ✔ Single table for the entire inheritance hierarchy.
            ✔ Discriminator column identifies entity type.
            ✔ Fastest query performance (no joins).
            ❌ NULL values.

            Example
                public class Employee
                {
                    public int Id { get; set; }
                    public string Name { get; set; }
                }

                public class FullTimeEmployee : Employee
                {
                    public decimal Salary { get; set; }
                }

                public class PartTimeEmployee : Employee
                {
                    public decimal HourlyRate { get; set; }
                }

                ❕Generated SQL
                CREATE TABLE Employees (
                    Id INT PRIMARY KEY,
                    Name NVARCHAR(MAX),
                    Salary DECIMAL(18,2), 
                    HourlyRate DECIMAL(18,2),
                    Discriminator NVARCHAR(MAX)
                        ❕Stores "Employee", "FullTimeEmployee", or "PartTimeEmployee"
                );

        [2] Table Per Type (TPT)
            ✔ Separate tables for base and derived types.
            ✔ No NULL fields.
            ❌ Performance cost due to JOINs.

            Example
                public class Employee
                {
                    public int Id { get; set; }
                    public string Name { get; set; }
                }

                public class FullTimeEmployee : Employee
                {
                    public decimal Salary { get; set; }
                }

                public class PartTimeEmployee : Employee
                {
                    public decimal HourlyRate { get; set; }
                }

                ❕Generated SQL
                CREATE TABLE Employees (
                    Id INT PRIMARY KEY,
                    Name NVARCHAR(MAX)
                );

                CREATE TABLE FullTimeEmployees (
                    Id INT PRIMARY KEY,
                    Salary DECIMAL(18,2),
                    FOREIGN KEY (Id) REFERENCES Employees(Id)
                );

                CREATE TABLE PartTimeEmployees (
                    Id INT PRIMARY KEY,
                    HourlyRate DECIMAL(18,2),
                    FOREIGN KEY (Id) REFERENCES Employees(Id)
                );

        [3] Table Per Concrete Type (TPC)
            ✔ Each concrete class has its own table.
            ✔ No discriminator column or foreign keys.
            ❌ No shared base table for common fields -> Leads to larger database size.

            Example
                public class Employee
                {
                    public int Id { get; set; }
                    public string Name { get; set; }
                }

                public class FullTimeEmployee : Employee
                {
                    public decimal Salary { get; set; }
                }

                public class PartTimeEmployee : Employee
                {
                    public decimal HourlyRate { get; set; }
                }

                ❕Generated SQL
                CREATE TABLE FullTimeEmployees (
                    Id INT PRIMARY KEY,
                    Name NVARCHAR(MAX),
                    Salary DECIMAL(18,2)
                );

                CREATE TABLE PartTimeEmployees (
                    Id INT PRIMARY KEY,
                    Name NVARCHAR(MAX),
                    HourlyRate DECIMAL(18,2)
                );


*/

// * Mapping
/*
    🗒️By Convention [DEFAULT]
        [1] Primary Key
            EF Core will look for a public numeric property named Id or <EntityName>Id and mark it as the primary key (PK).
            This PK will be marked as an Identity Column
                INT IDENTITY(1,1) PRIMARY KEY

            Example:
                public class User
                {
                    public int Id { get; set; } // PK by convention
                    public string Name { get; set; }
                }   

        [2] Foreign Keys & Relationships
            If a navigation property has a corresponding <NavigationPropertyName>Id, EF Core treats it as a foreign key.
            
            Example (One-to-Many):
            public class Order
            {
                public int Id { get; set; } // PK
                public int UserId { get; set; } // FK by convention
                public User User { get; set; } // Navigation property
            }

        [3] One-to-Many Relationship
            - A collection navigation property in one entity and a reference navigation property in the other define a one-to-many relationship.
            Example:
            public class User
            {
                public int Id { get; set; }
                public ICollection<Order> Orders { get; set; } // One-to-Many
            }

            public class Order
            {
                public int Id { get; set; }
                public int UserId { get; set; }
                public User User { get; set; }
            }

        [4] Many-to-Many Relationship
            - EF Core automatically creates a join table if both sides have collection navigation properties.
            Example:
            public class Student
            {
                public int Id { get; set; }
                public ICollection<Course> Courses { get; set; }
            }

            public class Course
            {
                public int Id { get; set; }
                public ICollection<Student> Students { get; set; }
            }

        [5] Nullable & Non-Nullable Properties

            Reference Types (string)
                In .NET 5, all string properties are optional (NVARCHAR(MAX) NULL).
                In .NET 6+,
                    non-nullable string (string Name) is required (NVARCHAR(MAX) NOT NULL).
                    nullable string (string? Name) is optional (NVARCHAR(MAX) NULL).

            Value Types (int, decimal)
                Non-nullable value types (int, decimal) → NOT NULL.
                Nullable value types (int?, decimal?) → NULL.

            Example
                public class Employee
                {
                    public int Id { get; set; }
                        PK → INT IDENTITY(1,1)

                    public string Name { get; set; }
                        .NET 5 → NVARCHAR(MAX) NULL [OPTIONAL]
                        .NET 6+ → NVARCHAR(MAX) NOT NULL [REQUIRED]

                    public string? Email { get; set; }
                        NVARCHAR(MAX) NULL [OPTIONAL]

                    public int Age { get; set; }
                        INT NOT NULL [REQUIRED]

                    public int? Bonus { get; set; }
                        INT NULL [OPTIONAL]

                    public decimal Salary { get; set; }
                        DECIMAL(18,2) NOT NULL [REQUIRED]
                }

*/

// * DbContext
/*
    Each database provider has its own version of DbContext
    You need to Install the database provider package to get an access to the required DbContext.
        SQL Server:	Microsoft.EntityFrameworkCore.SqlServer
        SQLite:	Microsoft.EntityFrameworkCore.Sqlite
        PostgreSQL:	Npgsql.EntityFrameworkCore.PostgreSQL
        MySQL: Pomelo.EntityFrameworkCore.MySql (community-supported)
        Oracle:	Oracle.EntityFrameworkCore

    DbContext is an abstract class, acts as a bridge between your application and the database:
        Manages Database Connections: Opens and closes connections automatically.
        Tracks Changes: Keeps track of entity states (Added, Modified, Deleted, UnChanged).
        Executes Queries: Translates LINQ queries into SQL and fetches data.
        Handles Transactions: Ensures data consistency through SaveChanges().
        Configures Entities: Defines relationships and constraints via Fluent API or Data Annotations.

    🗒️DbContext Inheritance & Core Methods

        public class AppDbContext : DbContext
        {
            ❕A DbSet<T>
                -> Public object-member property of the type that will be mapped to a table in database  
                public DbSet<Employee> Employees { get; set; }

            ❕OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                -> Configures the database provider and connection string.
                -> Used only if you're not using Dependency Injection (DI).

                protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                {
                    optionsBuilder.UseSqlServer("Server=.;Database=MyAppDB;Trusted_Connection=True;MultipleActiveResultSets=True;");
                    	Server=. -> Specifies the SQL Server instance.
                        Database=MyAppDB -> Specifies the database name.
                        Trusted_Connection=True; -> Uses Windows Authentication (instead of a username/password).
                        MultipleActiveResultSets=True; -> If you need to execute multiple SQL queries simultaneously within the same connection.
                }

            ❕OnModelCreating(ModelBuilder modelBuilder)
                -> Used for Fluent API configurations.
        }

*/

// * Migrations