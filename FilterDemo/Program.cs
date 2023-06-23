using FilterDemo;
using FilterDemo.Entities;

var customers = new List<Customer>
{
    new Customer { Id = 1, Name = "John Doe", Age = 30, Address = "123 Main St" },
    new Customer { Id = 2, Name = "Jane Smith", Age = 25, Address = "456 Elm St" },
    new Customer { Id = 3, Name = "Mike Johnson", Age = 40, Address = "789 Oak St" },
};

var filterParameters = new FilterRequest
{
    Filters = new()
    {
        new()
        {
            Name = "Age",
            Value = 30,
            Operator = FilterOperator.Equals
        },
        new()
        {
            Name = "Address",
            Value = "Main",
            Operator = FilterOperator.Contains
        }
    },
    Sort = new()
    {
        Name = "Name",
        Direction = SortDirection.Ascending
    },
    Select = new List<string> { "Id", "Name" },
    Page = 1,
    PageSize = 10,
    Count = true
};

var result = customers.AsQueryable()
    .ApplyFilters(filterParameters);

Console.WriteLine($"Filtered Customers (Page {result.Page}/{result.PageCount}):");

foreach (var customer in result.Data)
{
    Console.WriteLine($"ID: {customer.Id}, Name: {customer.Name}");
}