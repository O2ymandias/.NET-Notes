// * C# Collection Expression [.. IEnumerable]
/*
    Allows you to spread elements from an IEnumerable into a new collection.
    It's like the spread operator in JavaScript.
    Example:
        var list = new List<int> { 1, 2, 3 };
        var result = [0, ..list, 4]; // [0, 1, 2, 3, 4]

        string str = "abc";
        var chars = [..str]; // ['a', 'b', 'c']
*/