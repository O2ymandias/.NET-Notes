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

            ❕EF Core Mapping
                Need to be configured
                    protected override void OnModelCreating(ModelBuilder modelBuilder)
                    {   
                        [Optional]
                        modelBuilder
                            .Entity<Employee>()
                            .ToTable("Employees") -> rename the table
                            .HasDiscriminator(e => e.Disc); -> have a representation of the discriminator column
                            .HasDiscriminator<string>("Disc"); -> rename discriminator

                        .HasDiscriminator<string>("Disc"); // Optional: rename discriminator
                        
                        modelBuilder.Entity<FullTimeEmployee>().HasBaseType<Employee>();
                        modelBuilder.Entity<PartTimeEmployee>().HasBaseType<Employee>();
                    }

                We still can have DbSets for each type [Optional]  
                    public DbSet<FullTimeEmployee> FullTimeEmployees { get; set; }
                    public DbSet<PartTimeEmployee> PartTimeEmployees { get; set; }
                
                Or one DbSet for the base type [Optional]
                    public DbSet<Employee> Employees { get; set; }




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

            ❕EF Core Mapping
                DbSet for each concrete type
                	public DbSet<FullTimeEmployee> FullTimeEmployees { get; set; }
	                public DbSet<PartTimeEmployee> PartTimeEmployees { get; set; }



*/

// * Mapping

// 🗒️By Convention [DEFAULT]
/*
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


    [2] Nullable & Non-Nullable Properties

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

// 🗒️By Data Annotation
/*
    Attributes applied to model properties to configure how they should be mapped to the database.

    [Key] 
        Marks the property as PK.
        If the property is of type int or long, it will use identity constraint

    [DatabaseGenerated()]
        Controls how values are generated for a database column.
        it accepts a DatabaseGeneratedOption enum, which has three options:
            1. DatabaseGeneratedOption.Identity
                This makes the database auto-generate values for columns (using an identity constraint  IDENTITY(1,1))

            2. DatabaseGeneratedOption.Computed
                Marks a Property as Computed.
                It is NOT necessary if you use Fluent API (.HasComputedColumnSql()).
                Fluent API (.HasComputedColumnSql()) explicitly defines how the computation is done in SQL.
                The attribute is only useful when the database already has a computed column, and you want EF Core to recognize it without modifying it.

            3. DatabaseGeneratedOption.None
                The database does not auto-generate or compute the value.
                The application must manually assign a value before inserting a record.


    [Required]
        To explicitly make a column as required [NOT NULL] , to avoid .NET versions conflicts.
        public class Employee


    [StringLength()] & [MaxLength()]
        Both affects database scheme.
        
        StringLength()
        ASP.NET MVC/API: Validates user input


    [Column("ColumnName", TypeName = "SQLType")]
        If you want to change the column name or specify the data type


    [ForeignKey("NavigationProperty")]
        Defines foreign key relationships.

    [Table]
        If you want to explicitly specify a different table name (or schema),

    [NotMapped]
        specify that a particular property should not be mapped to a database column.



    🗒️Attributes doesn't affect the database scheme.
        [RegularExpression]
        [Compare]
        [Range]
        [Email]
        [Phone]
        [Url]
        [DataType]
        The MinimumLength of [StringLength]
*/

// 🗒️By Fluent APIS
/*
    Provides a way to configure entity properties, relationships, and constraints directly in the OnModelCreating method of DbContext.

    ❕Some API Methods
        .ToTable("TableName")
            Maps an entity to a specific table name.

        .Entity<EntityType>()
            .HasKey(x => x.Id)
                Defines the primary key.

        .Property(x => x.PropertyName) || .Property(nameof(Entity.PropertyName)) || .Property("PropertyName")
            .IsRequired(): Makes a property NOT NULL.
            .HasMaxLength(): Limits the length of a string column.
            .HasColumnName(): Maps a property to a specific column name.
            .UseIdentityColumn(): Configures the identity constraint.
            .HasDefaultValue():
                Use when setting a fixed, constant default value.
                The value is set at insert if no value is provided.

            .HasDefaultValueSql("GETDATE()"):
                Use when the default value should be generated dynamically using SQL.
                The value is computed at insert using a SQL function.

            .HasComputedColumnSql():
                Use when the column value should be calculated dynamically based on other columns.
                The database recalculates it on every query.


        .HasOne().WithMany()
            Configures one-to-many relationships.

        .HasForeignKey("FK")
            Specifies the foreign key column.

        .OnDelete(DeleteBehavior.Cascade)
            Configures delete behavior for related entities.
*/

// 🗒️Entity Type Configuration (IEntityTypeConfiguration<T>)
/*
    IEntityTypeConfiguration<T>: It is an interface provided by EF Core that allows you to define entity configurations outside of OnModelCreating

    You need to apply those configurations at OnModelCreating
        1. modelBuilder.ApplyConfiguration(new EntityConfig());
        2. modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            Applies configuration from all classes instances that implements IEntityTypeConfiguration that are defined in provided assembly.
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
                    optionsBuilder.UseSqlServer(
                    "Server=.;Database=MyAppDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
                    );
                    	(Server = .) -> Specifies the SQL Server instance.

                        (Database = MyAppDB) -> Specifies the database name.

                        (Trusted_Connection = True) -> Uses Windows Authentication (instead of a username/password).

                        (TrustServerCertificate = True)
                            -> Old Behavior(.NET 6): Encrypt = false
                            -> New Behavior(.NET 7): Encrypt = true
                                This means:
                                    1. The server (the machine running Microsoft SQL Server) must be configured with a valid certificate
                                    2. The client (ASP.NET Core App) must trust this certificate.


                        (MultipleActiveResultSets = True) -> (MARS) If you need to execute multiple SQL queries simultaneously within the same connection.
                }

            ❕OnModelCreating(ModelBuilder modelBuilder)
                -> Used for Fluent API configurations.
        }

    🗒️What Happens If You Don't Add a DbSet<T> in DbContext?
        If you don't add a DbSet<T> for an entity in your DbContext, EF Core will not track it by default, but it can still be mapped to the database if it's included in the model via the OnModelCreating() method.

*/

// * Migrations
/*
    Install Microsoft.EntityFrameworkCore.Tools Package
    Allows to
        1. Manage Migrations
        2. Scaffold a DbContext and entity types by reverse engineering the schema of a database.

    Commands:
        Add-Migration: Adds a new migration.
        Remove-Migration: Removes the last migration (only if it has NOT been applied to the database).
        Get-Migration: Lists available migrations.
        Update-Database: Updates the database to the last migration or to a specified migration.
        Drop-Database: Drops the database.
        Scaffold-DbContext: Generates a DbContext and entity type classes for a specified database.

    🗒️What happens when you add the first migration ?
        [1] EF Core generates a Migrations folder (if it doesn't already exist). This folder contains:
            1. YYYYMMDDHHMMSS_InitialCreate.cs (Migration File)
                -> Contains Up() and Down() methods.
                    Up()
                        Defines the changes EF Core will apply.
                        To apply: Update-Database

                    Down()
                        Defines how to undo those changes.
                        To apply (Reverting): Update-Database "Specific Migration"
                            ❕To revert the first migration -> Update-Database 0

            2. MyDbContextModelSnapshot.cs (Snapshot File)
                Keeps a record of the latest database schema
                EF Core compares your current model configuration against this snapshot to determine what's changed, then generates the appropriate migration code to update the database schema.

            ❕EF Core did't modify the database yet. It only prepares the migration.

        
*/

// * using & try-finally
/*
    using 
        It ensures that the object (Implements IDisposable) is disposed of/ cleaned up/ released at the end of the block, even if an exception occurs.
        DbContext in RF Core implements IDisposable.

        Example
            using (var dbContext = new AppDbContext())
            {
                CRUD Operations
            }

            More Syntactic Sugar
                using var dbContext = new AppDbContext();

    try-finally
        You manually call Dispose() in the finally block, ensuring the connection is closed even if an exception is thrown.
        This approach can be useful if you need to add extra logic before or after disposing of the resource

        Example
            var dbContext = new AppDbContext();
            try
            {
                CRUD Operations
            }
            finally
            {
                dbContext.Dispose();
                Ensure DbContext is disposed of even if an exception occurs
            }

*/

// * Entry State
/*
    Detached:  
        The entity is not being tracked by EF Core.
        It could either be a new entity that hasn't been added to the context or an entity that has been removed from tracking. 

    Unchanged:  
        The entity is being tracked by EF Core and exists in the database.
        Its property values have not changed from the values in the database. 

    Deleted 
        The entity is being tracked by EF Core and exists in the database.
        It has been marked for deletion from the database. 
        When SaveChanges() is called, EF Core will delete it from the database.

    Modified 
        The entity is being tracked by EF Core and exists in the database.
        Some or all its property values have been modified.
        when SaveChanges() is invoked, those changes are saved to the database. 

    Added 
        The entity is being tracked by EF Core but does not yet exist in the database.
        when SaveChanges() is called, it will be inserted into the database.
                
*/


// * Change Tracking
/*
    Change Tracker:
        EF Core includes a Change Tracker, which is responsible for keeping track of the state of entities 
        retrieved from the database or intended to be saved to it.

    It tracks operations like:
        Added
        Modified
        Deleted
        Unchanged
        Detached

    By default, EF Core tracks all entities returned from queries so it can detect and persist changes.


    🗒️WHY DISABLE CHANGE TRACKING?
        In scenarios where you only need to read data, tracking adds unnecessary overhead 
        (memory usage and processing time).


    🗒️You can improve performance in read-only scenarios by disabling tracking.

        Option [1]: Disable tracking on a per-query basis
            Use `.AsNoTracking()` to disable tracking for a specific query:
                var users = context.Users.AsNoTracking().ToList();

                `users` will not be tracked by the Change Tracker.
                Any changes made to them will not be saved to the database.
                Ideal for read-only queries.

        Option [2]: Disable tracking globally (DbContext level)
            You can set the tracking behavior in the `OnConfiguring` method of your `DbContext`:
                protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                {
                    optionsBuilder
                        .UseSqlServer("ConnectionString")
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                }

            This will disable tracking by default for all queries in the DbContext.
            You can still opt-in to tracking for specific queries using `.AsTracking()`:
                var trackedUsers = context.Users.AsTracking().ToList();

*/

// * Relationships
// 🗒️[1] One-to-Many (1:M)
/*
    🗒️By Convention
        public class Blog
        {
            public int Id { get; set; }

            ‼️Navigation Property [M]
            public ICollection<Post> Posts { get; set; } = new HashSet<Post>()

        ❕Best Practice
            It's recommended to use ICollection for "many" navigation properties because:
                1. It provides Add()/Remove() methods, and Count property.
                2. It extends IEnumerable, so it's iterable (e.g., foreach loop).
                3. It's flexible, you can use a List, HashSet, or any collection that implements ICollection.
                4.It's recommend to initialize the collection to avoid null reference exception.
                5. For further improvement, initialize it with a HashSet for faster lookups.
        }

        public class Post
        {
            public int Id { get; set; }

            ‼️FK [NavigationPropertyName][PrimaryKeyName]
            public int BlogId { get; set; } 
                ❕Actually, EF Core can figure it out by convention and will automatically create this column in the database.
                ❕It uses the convention [NavigationPropertyName][PrimaryKeyName] (e.g., BlogId) for the foreign key.
                ❕It's useful in the code to have a representation of the FK, which can be useful for search/filtering.

            ‼️Navigation Property [1]
            public Blog Blog { get; set; }

        }

    In a one-to-many relationship, EF Core can figure out the "one" side from the navigation property, and "many" side via the FK (foreign key) property, but only if it follows naming conventions.


    🗒️Data Annotations
        [ForeignKey(nameof(NavigationProperty))]
            Useful when you rename the FK and want to explicitly link it to the navigation property.


        [InverseProperty(nameof(NavigationPropertyOnTheOtherEntity))]
            Useful when you have multiple relationships to the same entity.
            tells EF Core: “This navigation property on this entity matches with that navigation property on the other entity.”

        Example
            public class User
            {
                public int Id { get; set; }

                [InverseProperty(nameof(Post.Author))]
                public ICollection<Post> AuthoredPosts { get; set; }

                [InverseProperty(nameof(Post.Editor))]
                public ICollection<Post> EditedPosts { get; set; }
            }

            public class Post
            {
                public int Id { get; set; }

                ❕You can apply [ForeignKey] on the FK property
                [ForeignKey(nameof(Author))]
                public int AuthorId { get; set; }

                public User Author { get; set; }

                ❕You can also apply [ForeignKey] on the navigation property
                [ForeignKey(nameof(EditorId))]
                public User Editor { get; set; }

                public int EditorId { get; set; }
            }


    🗒️Fluent API Or Entity Type Configuration [Recommended]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
		    modelBuilder
			.Entity<Employee>()
			.HasOne(e => e.Department)
			.WithMany(d => d.Employees)
			.HasForeignKey(e => e.DepartmentId)
			.OnDelete(DeleteBehavior.Cascade);
        }


*/


// 🗒️[2] Many-to-Many (M:M)
/*
    🗒️By Convention
        public class Student
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public ICollection<Course> Courses { get; set; } = new HashSet<Course>();
        } 

        public class Course
        {
            public int Id { get; set; }
            public string Title { get; set; }

            public ICollection<Student> Students { get; set; } = new HashSet<Student>();
        }

        EF Core will automatically generate a shadow join table (a table that exists in the database, but not as a C# class).
        This table will link StudentId <-> CourseId using a composite primary key.

        ❕When to Represent the Join Table as a Real Class?
            When you need to store extra data in the relationship like: EnrolledAt, Grade, Status, etc.

            public class Enrollment
            {
                public int StudentId { get; set; } 
                public Student Student { get; set; } -> NV(1) 

                public int CourseId { get; set; }
                public Course Course { get; set; } -> NV(1)

                public DateTime EnrolledAt { get; set; } -> Extra field
            }

            public class Student
            {
                public int Id { get; set; }
                public string Name { get; set; }

                public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>() -> NV(M)
            }

            public class Course
            {
                public int Id { get; set; }
                public string Title { get; set; }

                public ICollection<Enrollment> Enrollments { get; set; } = = new HashSet<Enrollment>() -> NV(M)
            }

            So the Many-to-Many relationship is split into two One-to-Many (1:M) relationships:
                Student (1) ---> (M) Enrollment (M) <--- (1) Course



    🗒️By Fluent API Or Entity Type Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Student>()
                .HasMany(s => s.Courses)
                .WithMany(c => c.Students)
        }

        ❕In case we want a representation of the shadow table in C# code
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                Configure composite primary key (MUST)
                    modelBuilder.Entity<Enrollment>()
                        .HasKey(e => new { e.StudentId, e.CourseId });

                Configure relationship: Enrollment -> Student (many-to-one)
                    modelBuilder.Entity<Enrollment>()
                        .HasOne(e => e.Student)
                        .WithMany(s => s.Enrollments)
                        .HasForeignKey(e => e.StudentId)
                        .IsRequired()
                        .OnDelete(DeleteBehavior.Cascade);

                Configure relationship: Enrollment -> Course (many-to-one)
                    modelBuilder.Entity<Enrollment>()
                        .HasOne(e => e.Course)
                        .WithMany(c => c.Enrollments)
                        .HasForeignKey(e => e.CourseId)
                        .IsRequired()
                        .OnDelete(DeleteBehavior.Cascade);
            }
*/

// * Loading Navigation Property Strategies
/*
    By default, EF Core will not automatically load navigation properties (related entities).
    You must explicitly include them using one of the following methods:

        [1] Eager Loading (using Include)
            Loads related entities as part of the initial query.
            ❌ Always performs JOINs even if the related data isn't needed.

            Example:
                var blog = context.Blogs
                    .Include(b => b.Posts)
                    .ThenInclude(p => p.Author)
                    .FirstOrDefault(b => b.BlogId == 1);

        [2] Lazy Loading
            EF Core loads the navigation property automatically when it's first accessed. not when the entity is initially loaded.
            ❌ More round-trips to the database.
                Each navigation property access can trigger a separate SQL query "N+1 query problem"
                    If you're iterating through 100 authors and accessing their books, you'll get 101 database queries!

            Requirements:
                1. Install Microsoft.EntityFrameworkCore.Proxies
                2. Enable proxies in DbContext:
                    optionsBuilder.UseLazyLoadingProxies();
                3. Mark Entities as 'public'
                    You still can use 'internal' BUT mark the assembly with [assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
                    
                4. Mark navigation properties as 'virtual'

        [3] Explicit Loading
            Manually load the related data when needed.
            ❌ More round-trips to the database.
                Each related entity or collection must be loaded with a separate query, which can affect performance.

            Example:
                var blog = context.Blogs.FirstOrDefault(b => b.BlogId == 1);

                For collection navigation property (M)
                    context.Entry(blog)
                        .Collection(b => b.Posts)
                        .Load();

                For reference navigation property (1)
                    context.Entry(blog)
                        .Reference(b => b.Author)
                        .Load();

            
*/

// * Mapping View 
/*
    [1] Create Empty Migration
        protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
                CREATE VIEW vw_AllDepartmentsWithItsEmployees AS
                SELECT e.Id AS EmployeeId, e.Name AS EmployeeName, d.Id AS DepartmentId, d.Name AS DepartmentName
                FROM Departments d LEFT OUTER JOIN Employees e
				ON d.Id = e.DepartmentId
			");
		}
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DROP VIEW vw_AllDepartmentsWithItsEmployees");
		}

    [2] Updating the Database
        Update-Database -Context YourDbContextName


    [3] Create Model Class
        ❕ Properties MUST be the same as the SQL view columns names. or can be configured via Fluent API.
        public class AllDepartmentsWithItsEmployees
        {
            public int EmpId { get; set; }
            public string EmpName { get; set; }
            public int DeptId { get; set; }
            public string DeptName { get; set; }
        }

    [4] Configure It in DbContext
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AllDepartmentsWithItsEmployees>(etb =>
            {
                etb.HasNoKey();
                ❕Important: Views don't have primary keys

                etb.ToView("vw_AllDepartmentsWithItsEmployees");
                ❕ Must match the SQL view name if it has been created first at the database.

                ❕In case The SQL view columns names are different from the model class properties names, you can configure them using Fluent API.

                    etb.Property(x => x.DeptId).HasColumnName("DepartmentId");

                    etb.Property(x => x.DeptName).HasColumnName("DepartmentName");

                    etb.Property(x => x.EmpId).HasColumnName("EmployeeId");

                    etb.Property(x => x.EmpName).HasColumnName("EmployeeName");
            });

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<AllDepartmentsWithItsEmployees> AllDepartmentsWithItsEmployees { get; set; }


    [5] Querying the View
        var result = context.AllDepartmentsWithItsEmployees.ToList();

*/

// * Raw SQL Queries
/*
    [1] SELECT -> (FromSQLRaw, FromSqlInterpolated)
        var employees = context.Employees.FromSqlRaw("SELECT * FROM Employees").ToList();

        var employees = context.Employees.FromSqlInterpolated($"SELECT * FROM Employees WHERE Name = {name}").ToList();

        var employees = context.Employees.FromSqlRaw("SELECT * FROM Employees WHERE Name = {0}", name).ToList();

    [2] DML (INSERT, UPDATE, DELETE) -> (ExecuteSqlRaw, ExecuteSqlInterpolated)
        context.Database.ExecuteSqlRaw("INSERT INTO Employees (Name) VALUES ('John Doe')");

        context.Database.ExecuteSqlInterpolated($"UPDATE Employees SET Name = {name} WHERE Id = {id}");

        context.Database.ExecuteSqlRaw("DELETE FROM Employees WHERE Id = {0}", id);

    [3] Stored Procedures
        context.Database.ExecuteSqlRaw("EXEC UpdateEmployeeSalary @EmployeeId, @NewSalary", new SqlParameter("@EmployeeId", employeeId), new SqlParameter("@NewSalary", newSalary));

        Or Use EF Core Power Tools to scaffold the stored procedure into a C# method.
*/