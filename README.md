FilterPartner Demo
==================

This is a sample code snippet demonstrating the usage of the FilterPartner library in .NET 7.

Code Example
------------

    
    using FilterPartner;
    using FilterPartner.Entities;
    using System;
    using System.Collections.Generic;
    
    // Arrange
    
    List<Customer> customers = new()
    {
        new() { Id = 1, Name = "John Doe", Age = 30, Address = "123 Main St" },
        new() { Id = 2, Name = "Jane Smith", Age = 25, Address = "456 Elm St" },
        new() { Id = 3, Name = "Mike Johnson", Age = 40, Address = "789 Oak St" },
    };
    
    FilterRequest rq = new()
    {
        Filters = new()
        {
            new()
            {
                Name = nameof(Customer.Age),
                Value = 30,
                Operator = FilterOperator.Equals
            },
            new()
            {
                Name = nameof(Customer.Address),
                Value = "Main",
                Operator = FilterOperator.Contains
            }
        },
        Sort = new()
        {
            Name = nameof(Customer.Name),
            Direction = SortDirection.Ascending
        },
        Select = new()
        {
            nameof(Customer.Id),
            nameof(Customer.Name)
        },
        Page = 1,
        PageSize = 10,
        Count = true
    };
    
    // Act
    
    var rs = rq.Apply(customers);
    
    // Assert
    
    Console.WriteLine($"Filtered Customers (Page {rs.Page}/{rs.PageCount}):");
    
    foreach (var customer in rs.Data)
    {
        Console.WriteLine($"ID: {customer.Id}, Name: {customer.Name}");
    }
      

The code snippet demonstrates the usage of the FilterPartner library in .NET 7. The FilterPartner library provides functionality for filtering, sorting, and paginating collections of objects based on specific criteria.

In this example, a list of `Customer` objects is created, representing a collection of customer data. The `FilterRequest` object `rq` is then initialized with the desired filter conditions, sorting criteria, selection fields, pagination options, and counting preference.

The `Filters` property of the `FilterRequest` object is set to a collection of `FilterCondition` objects. Each `FilterCondition` specifies the property name, value, and comparison operator for a specific filter condition. In this case, the code filters for customers with an age of 30 and an address containing the word "Main".

The `Sort` property of the `FilterRequest` object defines the sorting criteria. It specifies sorting by the `Name` property of the `Customer` object in ascending order.

The `Select` property determines the fields to include in the result set. It specifies the `Id` and `Name` properties of the `Customer` object to be selected.

The `Page` and `PageSize` properties control the pagination of the results. In this example, the code specifies that the first page of results should be retrieved, with a page size of 10.

The `Count` property is set to `true` to indicate that the total count of filtered results should be calculated.

The `Apply` method is called on the `FilterRequest` object, passing in the `customers` list. This method applies the specified filters, sorting, and pagination to the `customers` collection and returns a `FilterResult` object `rs` that contains the filtered and sorted subset of data.

Finally, the code snippet prints the filtered customers to the console, displaying their IDs and names. The page number and total page count are also shown.

Feel free to modify the code snippet and experiment with different filter conditions, sorting options, and pagination settings to explore the capabilities of the FilterPartner library.