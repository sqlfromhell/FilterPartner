using FilterDemo;
using FilterDemo.Entities;

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
    Select = new() {
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