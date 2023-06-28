FilterDemo
==========

This is a sample code snippet demonstrating the usage of filters in .NET 7.

Code Example
------------

    
    using FilterDemo;
    using FilterDemo.Entities;
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
      

The code snippet starts by importing the required namespaces and setting up the necessary using statements. It then creates a list of `Customer` objects, populating it with sample data.

Next, a `FilterRequest` object `rq` is created, representing the desired filters, sorting criteria, selection fields, pagination options, and counting preference. The `Filters` property is set to a collection of `FilterCondition` objects, where each object specifies the property name, value, and comparison operator for a specific filter condition. In this example, the code filters for customers with an age of 30 and an address containing the word "Main". The `Sort` property is set to specify sorting by the `Name` property in ascending order. The `Select` property specifies the fields to include in the result set.

The `Apply` method is then called on the `FilterRequest` object, passing in the `customers` list. This method performs the filtering, sorting, and pagination based on the provided criteria and returns a `FilterResult` object `rs`.

Finally, the code snippet prints the filtered customers to the console, displaying their IDs and names. The `Page` and `PageCount` properties of the `FilterResult` object are used to show the current page number and the total number of pages.

Feel free to modify the code snippet and experiment with different filters, sorting options, and pagination settings to observe the effects on the filtered results.