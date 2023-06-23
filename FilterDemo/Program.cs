using FilterDemo;

// Sample list of customers
var customers = new List<Customer>
    {
        new Customer { Id = 1, Name = "John Doe", Age = 30, Address = "123 Main St" },
        new Customer { Id = 2, Name = "Jane Smith", Age = 25, Address = "456 Elm St" },
        new Customer { Id = 3, Name = "Mike Johnson", Age = 40, Address = "789 Oak St" },
        // Add more customers as needed
    };

// Example filter parameters
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

// Apply filters to the customer list
var filteredResult = customers.AsQueryable()
    .ApplyFilters(filterParameters);

// Access the filtered data and other information
var filteredCustomers = filteredResult.Data;
var totalCount = filteredResult.TotalCount;
var currentPage = filteredResult.Page;
var pageSize = filteredResult.PageSize;

// Display the filtered results
Console.WriteLine($"Filtered Customers (Page {currentPage}/{Math.Ceiling((double)totalCount / pageSize)}):");

foreach (var customer in filteredCustomers)
{
    Console.WriteLine($"ID: {customer.Id}, Name: {customer.Name}");
}