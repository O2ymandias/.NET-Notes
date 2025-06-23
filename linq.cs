// * var VS dynamic
/*
    [1] var (Implicitly Typed Variable)
        1. Compiler can detect data type based on the initial value at (Compilation Time).
        2. Must be initialized.
        3. Can't be initialized with null (null itself doesn't specify a type, so compiler can't detect data type).
        4. Can't change its data type after initialization.
        5. Performance: No runtime overhead since type checking happens at compile time.

        Example
            var number = 10; ❕Compiler infers "int".
            var number; ❌ Error: Must be initialized.
            var number = null ❌ Error: Can't be initialized with null.
            number = "10" ❌ Error: Can't change its data type after initialization.

    [2] dynamic (Dynamic Type)
        1. Compiler will skip type checking at (Compilation Time).
        2. CLR will resolve the actual type of the dynamic time at (RunTime).
        3. Does not need to be initialized.
        4. Can be initialized with null.
        5. Can be reassigned to any data type.
        6. Performance overhead due to runtime type resolution.

        Example
            dynamic value = 10; ❕Initially an int.
            value = "Hello"; ❕Now a string (No compile-time error).
            dynamic value = null; ❕Can be initialized with null.
            dynamic value; ❕Does not need to be initialized.
*/

// * Extension Method
/*
    ❕Allows you to add new methods to an existing type without modifying the original type.

    🗒️How to Create an Extension Method?
        1. The method must be static.
        2. It must be inside a (non-generic, static) class.
        3. The first parameter should use this keyword followed by the type you want to extend.
        4. You can call it like an instance method on objects of that type.

    🗒️Example
        static class IntExtensions
        {
            public static int Reverse(this int number) ‼️(this int) -> Extends type int 
            {
                int reversedNumber = 0, reminder = 0;

                while (number != 0)
                {
                    reminder = number % 10;
                    reversedNumber = reversedNumber * 10 + reminder;
                    number /= 10;
                }

                return reversedNumber;
            }
        }

        int x = 12345;
		int reversedX = x.Reverse();
*/

// * Anonymous Type
/*
    Temporary, lightweight object type that allows you to create an object without explicitly defining a class.
    It is useful when you need to store data quickly without creating a separate model class.


    🗒️Key Features:
        1. Defined using new { } syntax.
            var employeeOne = new {Id = "123", Name = "Nael", Salary = 10_000}

            var employeeTwo = new {Id = "321", Name = "Max", Salary = 9_999}
            ❕Will the compiler generate a new type ?
                Compiler reuses the same anonymous type as long as the signature (property names, order, and data types) remains identical.

                Console.WriteLine(employeeOne.GetType() == employeeTwo.GetType()); ‼️True

        2. Properties are read-only (Immutable).
            var employeeOne = new {Id = "123", Name = "Nael", Salary = 10_000}
            employeeOne.Id = "999" ❌
            ❕ This is how you can mutate it:
                employeeOne = employeeOne with {Id = "999"}
            
        3. The type is inferred at compile-time.

        4. Typically used with LINQ queries.
*/

// * LINQ Intro
/*
    ❕Stand for Language Integrated Query 
    ❕40+ extension methods to IEnumerable<T> interface, existing at Enumerable class 
    ❕You can use LINQ operators against data (Stored as Sequence) regardless of the data store [SQL Server / Oracle / MYSQL] 

    🗒️Sequence
        Object from any class that implements IEnumerable [List, Array, Dictionary]

        ❕There are two types of sequence: 
            Local Sequence (In memory collection):
                L2O (LINQ To Object): Works on collections like List<T>, Array, Dictionary<K,V>.
                L2XML (LINQ To XML): Used for querying and manipulating XML documents.
 

            Remote Sequence (External Data Sources): 
                ✔ L2E (LINQ to Entities): Works with Entity Framework to query databases.


                

    🗒️Syntax
        [1] Fluent Syntax
            1.1 As a static methods through Enumerable class
                var odds = Enumerable.Where(numbers, number => number % 2 == 1);

            1.2 As extension methods
                var odds = numbers.where(number => number % 2 == 1)

        [2] Query Expression
            Always begins with a (from) clause that introduces a range variable (means a variable that represents each element of the sequence).
            Can include various clauses like (where, orderby, join)
            Ends with either (select or group by).

            Example
                var odds = from number in numbers
                where number % 2 == 1
                select number;

    🗒️LINQ Execution
        ❕Deferred Execution
            The query execution is delayed until the sequence is iterated.
            Works on the latest state of the sequence. 
            All LINQ operators are deferred except: 
                1. Element Operators (First(), Last(), Single(), etc.)
                2. Aggregate Operators (Count(), Sum(), Average(), etc.)
                3. Casting Operators (ToList(), ToArray(), ToDictionary())

            Example
                var numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                var odds = numbers.Where(n => n % 2 == 1);
                numbers.AddRange([11, 12, 13, 14, 15]);
                foreach (var number in odds)
                    Console.WriteLine(number); // 1,3,5,7,9,11,13,15

        Immediate Execution
            The query executes immediately and stores the result.
            Does NOT reflect later changes to the source collection. 
            Forced by (Casting/Element/Aggregate) operators.

            Example
                var numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                var odds = numbers.Where(n => n % 2 == 1).ToList();
                numbers.AddRange([11, 12, 13, 14, 15]);
                foreach (var number in odds)
                    Console.WriteLine(number); // 1,3,5,7,9

    

*/

// * LINQ Operators
/*
    🗒️Filtration (Restrictions)
        Where()
        OfType()

    🗒️Transformation (Projection) 
        Select()
        SelectMany()
        Zip()

    🗒️Sorting (Ordering)
        Order() & OrderDescending()
            Type must implement IComparable interface OR Pass an instance of a class that implements IComparer<T> interface
        OrderBy() 
        OrderByDescending() 

        Ordering By Multiple Columns
            Only valid with fluent syntax. 
            ThenBy() 
            ThenByDescending()
        Reverse()

    🗒️Element Operators (Immediate Execution)
        Returns one value.
        Only valid with fluent syntax.
        First()
        Last()
        FirstOrDefault()
        LastOrDefault()
        ElementAt()
        ElementAtOrDefault()
        Single()
        SingleOrDefault()
            If it runs against remote sequence, It will be translated to Top 2 in SQL. 

    🗒️Aggregate Operators (Immediate Execution)
        Returns one value.
        Count()
            ❕What is the difference between Count property & Count()
                [1] Count Property
                    Applies to: sequences that implement ICollection<T> (e.g., List<T>, HashSet<T>, Dictionary<T>).

                    Performance: O(1) (Constant time) because it directly returns the stored count.

                [2] Count()
                    Applies to: Any IEnumerable<T> (including IQueryable<T> for LINQ-to-SQL).
                    Performance:
                        O(1) if the underlying sequence implements ICollection<T> (it internally uses .Count property).

                        O(n) (Iterates through the entire sequence) if the sequence does not implement ICollection<T>.

        Max()
            If selector provided (x => x.propertyName) 
                Equivalent to Select(x => x.propertyName).Max()
            If NOT
                Type MUST implement IComparable Interface (no selector provided).
        Min()
        MaxBy()
            Equivalent to OrderByDescending().FirstOrDefault(); 
        MinBy()
        Sum() 
        Average() 
        Aggregate()
            [1] Aggregate(
                ❕Func<AccumulatorValue, NextElement, TResult>
                    1st Param: Accumulator Value, If no seed provided, it will be assigned to the first element of the sequence.
                    2nd Param: The next element of the sequence.
                    TResult: Will be the next accumulator value.
            )   

            [2] Aggregate(
                ❕TAccumulate seed,
                    seed: Initial Accumulator Value

                ❕Func<AccumulatorValue, NextElement, TResult>
                    1st Param: Already initialized with seed value.
                    2nd Param: The next element of the sequence.
                    TResult: Will be the next accumulator value.
            )
            
            [3] Aggregate(
                ❕TAccumulate seed,
                ❕Func<AccumulatorValue, NextElement, TResult>,
                ❕Func<FinalAccumulatorValue, TResult>
                    Transforms the final accumulated value before returning it.
            )

    🗒️Casting Operators (Immediate Execution)
        ToList() 
        ToArray() 
        ToDictionary(Func<T, TKey> keySelector) 
        ToHashSet()

    🗒️Generator Operators
        These methods don't work on a sequence, the only way to call them is as static methods via Enumerable class 
        Range() 
        Repeat() 
        Empty()

    🗒️Set Operators
        Union() 
            Removes duplications
        Concat() [Union All] 
            Doesn't remove duplications 
            Union() = Concat() + Distinct() 
        Intersect()  
        Except() [Minus]

        UnionBy()
        IntersectBy()
        ExceptBy()

    ❕Default Equality Comparer in LINQ
        Set operators (Union(), Intersect(), and Except()) use default equality comparer to compare values.

        For Value Types (int, string, etc.)
            Comparisons are made based on values.

        For Reference Types (Objects, Classes)
            Comparisons are based on reference (memory address).
            Two objects are considered equal only if they share the same reference.
            If you want custom comparison you can
                -> Override Equals() and GetHashCode().
                    If the comparison will always between same type, it's recommended to implement IEquatable<T> Interface.

                -> Pass an obj of a class that implements IEqualityComparer to the set operator

                -> Use
                    UnionBy()
                    IntersectBy()
                    ExceptBy()


    🗒️Quantifiers Operators 
        Returns boolean value. 
        Any()
            ❕Which is better any() or count() > 0
                ❕If the sequence is doesn't implement ICollection<T>
                    Any() is more efficient.
                        Any(): It short-circuits and returns as soon as it finds the first element
                        Count() will iterate through the entire collection to count the elements.

                ❕If the sequence implements ICollection
                    .Count/Length: is the most efficient (Direct property access O(1))
                    Count(): is efficient because it simply returns .Count (also O(1)).
                    Any(): is efficient because it returns .Count != 0 (also O(1)). 

        All() 
        SequenceEquals () 
            Returns True if the two sequences are of equal length AND the corresponding elements are equal according to default equality comparer, otherwise False 

    🗒️Grouping Operators
        GroupBy()

    🗒️Partitioning Operators
        Take() 
        TakeLast() 
        Skip() 
        SkipLast() 
        TakeWhile() 
        SkipWhile() 

    🗒️Join Operators

        [1] Inner Join (Join() Operator)
            ❕Fluent Syntax
                var result = dbContext.Set<Department>()
                .Join(
                    dbContext.Set<Employee>(),
                    d => d.Id,
                    e => e.DepartmentId,
                    (department, employee) => new
                    {
                        Department = department,
                        Employee = employee
                    }
                );

            ❕Query Expression
                result = from department in dbContext.Set<Department>()
                        join employee in dbContext.Set<Employee>()
                        on department.Id equals employee.DepartmentId
                        select new
                        {
                            Department = department,
                            Employee = employee
                        };

                Or

                result = from d in dbContext.Set<Department>()
                        from e in dbContext.Set<Employee>()
                        where d.Id == e.DepartmentId
                        select new
                        {
                            Department = d,
                            Employee = e
                        };



        [2] Left Outer Join (GroupJoin() Operator)
            ❕Fluent Syntax
                var result = dbContext.Set<Department>()
                .GroupJoin(
                    dbContext.Set<Employee>(),
                    d => d.Id,
                    e => e.DepartmentId,
                    (department, employeeGroups) => new
                    {
                        Department = department,
                        Employees = employeeGroups.DefaultIfEmpty()
                    }
                )
                .SelectMany(
                    x => x.Employees,
                    (x, employee) => new
                    {
                        Department = x.Department,
                        Employee = employee
                    }
                );

                🧠How It Works
                    GroupJoin() creates a collection of employees for each department.
                    DefaultIfEmpty() ensures that if a department has no employees, it will still be included in the result with a null employee.
                    SelectMany() flattens the collection of employees into a single sequence.


            ❕Query Expression
                var result = from department in dbContext.Set<Department>()
                        join employee in dbContext.Set<Employee>()
                        on department.Id equals employee.DepartmentId into employeeGroups 
                        from employee in employeeGroups.DefaultIfEmpty()
                        select new
                        {
                            Department = department,
                            Employee = employee
                        };

                🧠How It Works
                    join ... into
                        creates a group join (IEnumerable of employees per department).

                    from employee in employeeGroups.DefaultIfEmpty()
                        flattens that group and ensures that each department is represented even if it has no employees.
                        If a department has no employees, the employee variable will be null for that department.

                    The result is:
                        One row per department-employee pair
                        Or one row per department with null employee.


        [3] CrossJoin
            ❕Fluent Syntax
                var result = dbContext.Set<Department>()
                    .SelectMany(d => dbContext.Set<Employee>().Select(e => new
                    {
                        Department = d,
                        Employee = e
                    }));
            ❕Query Expression
                result = from d in dbContext.Set<Department>()
                        from e in dbContext.Set<Employee>()
                        select new
                        {
                            Department = d,
                            Employee = e
                        };

*/

// * (let & into) Query Expression
/*
    [1] let
        allows you to create a temporary variable inside a query
        Example
            List<string> names = ["Nael", "Aliaa", "Aya", "Hossam", "Aisha"];

            var result = from name in names
                        ❕Creating temporary variable
                        let noVowel = Regex.Replace(name, "[aeiouAEIOU]", string.Empty)
                        where noVowel.Length > 1
                        select new { Name = name, NoVowelName = noVowel };

    [2] into
        allows you to continue the query after (select or group by)
        Example
            List<string> names = ["Nael", "Aliaa", "Aya", "Hossam", "Aisha"];

            var result = from name in names
                        select Regex.Replace(name, "[aeiouAEIOU]", string.Empty)
                        ❕Continuing the query after select
                        into noVowel
                        ❕No access to "name" anymore.
                        where noVowel.Length > 1
                        select noVowel;
*/

// * The Difference Between IEnumerable<T>, IQueryable<T>, IReadOnlyList<T>, and ICollection<T>
/*
    [1] IEnumerable<T>
        -> Operates on in-memory collections (L2O - LINQ to Objects).

        -> Supports deferred execution on in-memory data.
            Example:
                List<string> users = new() { "Nael", "Aliaa", "Aya", "Doaa" };
                IEnumerable<string> filteredUsers = users.Where(u => u.Length > 3);

        -> Supports foreach loops.

        -> Does not support indexing (no users[i]) → Not suitable for `for` loops.

        -> When used with EF Core, execution occurs on the client side *after* all data is fetched:
            Example:
                var users = context.Users       // IQueryable<User>
                             .ToList()         // Immediate execution → loads all users into memory
                             .Where(u => u.Age > 30); // Filtering happens in memory

            ❕All users are loaded into memory first, and only then the filtering occurs.


    [2] IQueryable<T>
        -> Operates on remote data sources (L2E - LINQ to Entities).

        -> Supports deferred execution — the query is not executed until it is iterated or materialized 
           (using terminal operators like ToList(), First(), Count()).

        -> Builds expression trees and translates the query into SQL, executing on the database server.

            Example:
                var users = context.Users            // IQueryable<User>
                               .Where(u => u.Age > 30)  // Translated into SQL WHERE clause
                               .ToList();               // Immediate execution → retrieves filtered users

            ❕Only users with Age > 30 are retrieved from the database.


    [3] IReadOnlyList<T>
        -> Represents a read-only collection of elements.

        -> Supports indexing (users[i]) → Suitable for `for` loops and random access.

        -> Does not support modification (no Add, Remove, or Clear methods).

        -> Often used for encapsulation — exposes data without allowing consumers to modify it.

            Example:
                IReadOnlyList<string> names = new List<string> { "Nael", "Aliaa", "Aya" };
                var first = names[0]; // Access by index
                // names.Add("Doaa"); ❌ Compile-time error


    [4] ICollection<T>
        -> Inherits from IEnumerable<T> and extends it with:
            Methods: Add(), Remove(), Clear()
            Property: Count

        -> Represents a modifiable collection of objects.

        -> Does not guarantee index access — while implementations like List<T> support indexing,
           ICollection<T> itself does *not* define indexers.

        -> Supports foreach loops.

        -> Allows adding and removing elements.
            Example:
                ICollection<string> names = new List<string> { "Nael", "Aliaa", "Aya" };
                names.Add("Hossam");  // Modifies the collection
*/
