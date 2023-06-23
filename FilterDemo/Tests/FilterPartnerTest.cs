using FilterDemo.Entities;
using NUnit.Framework;
using System.Linq.Expressions;

namespace FilterDemo.Tests;

[TestFixture]
public class CustomerFilterTests
{
    private readonly List<Customer> Customers
        = new()
        {
            new Customer { Id = 1, Name = "John Doe", Age = 30, Address = "123 Main St" },
            new Customer { Id = 2, Name = "Jane Smith", Age = 25, Address = "456 Elm St" },
            new Customer { Id = 3, Name = "Mike Johnson", Age = 40, Address = "789 Oak St" },
            new Customer { Id = 4, Name = "Alice Cooper", Age = 30, Address = "321 Pine St" },
        };

    [Test]
    public void Should_Filter_By_Contains()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Name = "Address",
                        Value = "Main",
                        Operator = FilterOperator.Contains
                    }
                }
        };

        // Act

        var filteredCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        // Assert

        Assert.AreEqual(1, filteredCustomers.Count);
        Assert.IsTrue(filteredCustomers.All(c => c.Address.Contains("Main")));
    }

    [Test]
    public void Should_Filter_By_EndsWith()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Name = "Name",
                        Value = "Smith",
                        Operator = FilterOperator.EndsWith
                    }
                }
        };

        // Act

        var filteredCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        // Assert

        Assert.AreEqual(1, filteredCustomers.Count);
        Assert.IsTrue(filteredCustomers.All(c => c.Name.EndsWith("Smith")));
    }

    [Test]
    public void Should_Filter_By_Equality()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Name = "Age",
                        Value = 30,
                        Operator = FilterOperator.Equals
                    }
                }
        };

        // Act

        var filteredCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        // Assert

        Assert.AreEqual(2, filteredCustomers.Count);
        Assert.IsTrue(filteredCustomers.All(c => c.Age == 30));
    }

    [Test]
    public void Should_Filter_By_GreaterThan()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Name = "Age",
                        Value = 30,
                        Operator = FilterOperator.GreaterThan
                    }
                }
        };

        // Act

        var filteredCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        // Assert

        Assert.AreEqual(1, filteredCustomers.Count);
        Assert.IsTrue(filteredCustomers.All(c => c.Age > 30));
    }

    [Test]
    public void Should_Filter_By_GreaterThanOrEqual()
    {
        var filterParameters = new FilterRequest
        {
            Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Name = "Age",
                        Value = 30,
                        Operator = FilterOperator.GreaterThanOrEqual
                    }
                }
        };

        // Act

        var filteredCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        // Assert

        Assert.AreEqual(3, filteredCustomers.Count);
        Assert.IsTrue(filteredCustomers.All(c => c.Age >= 30));
    }

    [Test]
    public void Should_Filter_By_In()
    {
        // Arrange

        var ages = new[] { 25, 40 };

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = "Age",
                    Value = ages,
                    Operator = FilterOperator.In
                }
            }
        };

        // Act

        var rs = Customers.AsQueryable()
            .ApplyFilters(rq).Data
            .ToList();

        // Assert

        Assert.That(rs, Has.Count.EqualTo(2));

        Assert.That(rs.All(c => ages.Contains(c.Age)), Is.True);
    }

    [Test]
    public void Should_Filter_By_In_List()
    {
        // Arrange

        List<int> ages = new() { 25, 40 };

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = "Age",
                    Value = ages,
                    Operator = FilterOperator.In
                }
            }
        };

        // Act

        var rs = Customers.AsQueryable()
            .ApplyFilters(rq).Data
            .ToList();

        // Assert

        Assert.That(rs, Has.Count.EqualTo(2));

        Assert.That(rs.All(c => ages.Contains(c.Age)), Is.True);
    }

    [Test]
    public void Should_Filter_By_Inequality()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Name = "Age",
                        Value = 30,
                        Operator = FilterOperator.NotEquals
                    }
                }
        };

        // Act

        var filteredCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        // Assert

        Assert.AreEqual(2, filteredCustomers.Count);
        Assert.IsTrue(filteredCustomers.All(c => c.Age != 30));
    }

    [Test]
    public void Should_Filter_By_LessThan()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Name = "Age",
                        Value = 30,
                        Operator = FilterOperator.LessThan
                    }
                }
        };

        // Act

        var filteredCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        // Assert

        Assert.AreEqual(1, filteredCustomers.Count);
        Assert.IsTrue(filteredCustomers.All(c => c.Age < 30));
    }

    [Test]
    public void Should_Filter_By_LessThanOrEqual()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Name = "Age",
                        Value = 30,
                        Operator = FilterOperator.LessThanOrEqual
                    }
                }
        };

        // Act

        var filteredCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        // Assert

        Assert.AreEqual(3, filteredCustomers.Count);
        Assert.IsTrue(filteredCustomers.All(c => c.Age <= 30));
    }

    [Test]
    public void Should_Filter_By_NotContains()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Name = "Address",
                        Value = "St",
                        Operator = FilterOperator.NotContains
                    }
                }
        };

        // Act

        var filteredCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        // Assert

        Assert.AreEqual(0, filteredCustomers.Count);
        Assert.IsFalse(filteredCustomers.Any(c => c.Address.Contains("St")));
    }

    [Test]
    public void Should_Filter_By_NotIn()
    {
        // Arrange

        var ages = new[] { 25, 40 };

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = "Age",
                    Value = ages,
                    Operator = FilterOperator.NotIn
                }
            }
        };

        // Act

        var rs = Customers.AsQueryable()
            .ApplyFilters(rq).Data
            .ToList();

        // Assert

        Assert.That(rs, Has.Count.EqualTo(2));

        Assert.That(rs.All(c => !ages.Contains(c.Age)), Is.True);
    }

    [Test]
    public void Should_Filter_By_NotIn_List()
    {
        // Arrange

        var ages = new List<int> { 25, 40 };

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = "Age",
                    Value = ages,
                    Operator = FilterOperator.NotIn
                }
            }
        };

        // Act

        var rs = Customers.AsQueryable()
            .ApplyFilters(rq).Data
            .ToList();

        // Assert

        Assert.That(rs, Has.Count.EqualTo(2));

        Assert.That(rs.All(c => !ages.Contains(c.Age)), Is.True);
    }

    [Test]
    public void Should_Filter_By_StartsWith()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Name = "Name",
                        Value = "John",
                        Operator = FilterOperator.StartsWith
                    }
                }
        };

        // Act

        var filteredCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        // Assert

        Assert.AreEqual(1, filteredCustomers.Count);
        Assert.IsTrue(filteredCustomers.All(c => c.Name.StartsWith("John")));
    }

    [Test]
    public void Should_Filter_Using_CustomExpression()
    {
        // Arrange

        static bool func(Customer c) => c.Age > 30;

        Expression<Func<Customer, bool>> exp = c => c.Age > 30;

        var filterParameters = new FilterRequest
        {
            CustomFilters = new List<CustomFilterExpression>
                {
                    new CustomFilterExpression
                    {
                        Expression = exp
                    }
                }
        };

        // Act

        var filteredCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        // Assert

        Assert.AreEqual(1, filteredCustomers.Count);
        Assert.IsTrue(filteredCustomers.All(func));
    }

    [Test]
    public void Should_Get_TotalCount_Without_Pagination()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Count = true
        };

        // Act

        var totalCount = Customers.AsQueryable().ApplyFilters(filterParameters).TotalCount;

        // Assert

        Assert.AreEqual(4, totalCount);
    }

    [Test]
    public void Should_Paginate_Results()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Page = 2,
            PageSize = 2,
            Count = true
        };

        // Act

        var paginatedResult = Customers.AsQueryable().ApplyFilters(filterParameters);

        // Assert

        Assert.AreEqual(2, paginatedResult.Data.Count);
        Assert.AreEqual(2, paginatedResult.Page);
        Assert.AreEqual(2, paginatedResult.PageSize);
        Assert.AreEqual(4, paginatedResult.TotalCount);
    }

    [Test]
    public void Should_Select_Properties()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Select = new List<string> { "Id", "Name" }
        };

        // Act

        var selectedCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        // Assert

        Assert.AreEqual(4, selectedCustomers.Count);
        Assert.IsTrue(selectedCustomers.All(c => c.Age == 0 && c.Address == null));
    }

    [Test]
    public void Should_Sort_By_Name_Ascending()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Sort = new SortExpression
            {
                Name = "Name",
                Direction = SortDirection.Ascending
            }
        };

        // Act

        var sortedCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        var expectedOrder = Customers.OrderBy(c => c.Name).ToList();

        // Assert

        Assert.AreEqual(expectedOrder, sortedCustomers);
    }

    [Test]
    public void Should_Sort_By_Name_Descending()
    {
        // Arrange

        var filterParameters = new FilterRequest
        {
            Sort = new SortExpression
            {
                Name = "Name",
                Direction = SortDirection.Descending
            }
        };

        // Act

        var sortedCustomers = Customers.AsQueryable().ApplyFilters(filterParameters).Data.ToList();

        var expectedOrder = Customers.OrderByDescending(c => c.Name).ToList();

        // Assert

        Assert.AreEqual(expectedOrder, sortedCustomers);
    }
}