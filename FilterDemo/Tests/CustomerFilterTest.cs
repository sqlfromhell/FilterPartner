using System.Linq.Expressions;
using System.Text.Json;
using NUnit.Framework;

namespace FilterDemo.Tests;

[TestFixture]
public class CustomerFilterTest
{
    private readonly List<Customer> Customers =
        new() {
            new() {
                Id = 1,
                Name = "John Doe",
                Age = 30,
                Address = "123 Main St",
                BirthDate = new(1986, 12, 2)
            },
            new() {
                Id = 2,
                Name = "Jane Smith",
                Age = 25,
                Address = "456 Elm St",
                BirthDate = new(1991, 8, 2)
            },
            new() {
                Id = 3,
                Name = "Mike Johnson",
                Age = 40,
                Address = "789 Oak St"
            },
            new() {
                Id = 4,
                Name = "Alice Cooper",
                Age = 30,
                Address = "321 Pine St"
            },
        };

    [Test]
    public void Should_Apply()
    {
        // Arrange

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
                nameof(Customer.Age),
                nameof(Customer.Address)
            },
            Page = 1,
            PageSize = 10,
            Count = true
        };

        // Act

        var rs = rq.Apply(Customers);

        // Assert

        Assert.That(rs.TotalCount, Is.EqualTo(1));

        Assert.That(rs.PageCount, Is.EqualTo(1));

        Assert.That(rs.PageSize, Is.EqualTo(10));

        Assert.That(rs.Data, Has.Count.EqualTo(1));

        Assert.That(rs.Data.All(c => c.Name is null), Is.True);

        Assert.That(rs.Data.All(c => c.Age == 30), Is.True);

        Assert.That(rs.Data.All(c => c.Address.Contains("Main")), Is.True);
    }

    [Test]
    public void Should_Apply_When_LowerCase()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Age).ToLower(),
                    Value = 30,
                    Operator = FilterOperator.Equals
                },
                new()
                {
                    Name = nameof(Customer.Address).ToLower(),
                    Value = "Main",
                    Operator = FilterOperator.Contains
                }
            },
            Sort = new()
            {
                Name = nameof(Customer.Name).ToLower(),
                Direction = SortDirection.Ascending
            },
            Select = new() {
                nameof(Customer.Id).ToLower(),
                nameof(Customer.Address).ToLower(),
                nameof(Customer.Age).ToLower()
            },
            Page = 1,
            PageSize = 10,
            Count = true
        };

        // Act

        var rs = rq.Apply(Customers);

        // Assert

        Assert.That(rs.TotalCount, Is.EqualTo(1));

        Assert.That(rs.PageCount, Is.EqualTo(1));

        Assert.That(rs.PageSize, Is.EqualTo(10));

        Assert.That(rs.Data, Has.Count.EqualTo(1));

        Assert.That(rs.Data.All(c => c.Name is null), Is.True);

        Assert.That(rs.Data.All(c => c.Age == 30), Is.True);

        Assert.That(rs.Data.All(c => c.Address.Contains("Main")), Is.True);
    }

    [Test]
    public void Should_Filter_By_Contains()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Address),
                    Value = "Main",
                    Operator = FilterOperator.Contains
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(1, rs.Count);
        Assert.IsTrue(rs.All(c => c.Address.Contains("Main")));
    }

    [Test]
    public void Should_Filter_By_Contains_Json()
    {
        // Arrange

        var json = JsonDocument.Parse("\"Main\"")
            .RootElement;

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Address),
                    Value = json,
                    Operator = FilterOperator.Contains
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(1, rs.Count);
        Assert.IsTrue(rs.All(c => c.Address.Contains("Main")));
    }

    [Test]
    public void Should_Filter_By_EndsWith()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Name),
                    Value = "Smith",
                    Operator = FilterOperator.EndsWith
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(1, rs.Count);
        Assert.IsTrue(rs.All(c => c.Name.EndsWith("Smith")));
    }

    [Test]
    public void Should_Filter_By_Equality()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Age),
                    Value = 30,
                    Operator = FilterOperator.Equals
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(2, rs.Count);
        Assert.IsTrue(rs.All(c => c.Age == 30));
    }

    [Test]
    public void Should_Filter_By_Equality_BirthDate()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.BirthDate),
                    Value = "1986-12-02T00:00:00",
                    Operator = FilterOperator.Equals
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(1, rs.Count);
        Assert.IsTrue(rs.All(c => c.BirthDate == new DateTime(1986, 12, 2)));
    }

    [Test]
    public void Should_Filter_By_Equality_Json()
    {
        // Arrange

        var json = JsonDocument.Parse("30")
            .RootElement;

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Age),
                    Value = json,
                    Operator = FilterOperator.Equals
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(2, rs.Count);
        Assert.IsTrue(rs.All(c => c.Age == 30));
    }

    [Test]
    public void Should_Filter_By_GreaterThan()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Age),
                    Value = 30,
                    Operator = FilterOperator.GreaterThan
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(1, rs.Count);
        Assert.IsTrue(rs.All(c => c.Age > 30));
    }

    [Test]
    public void Should_Filter_By_GreaterThanOrEqual()
    {
        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Age),
                    Value = 30,
                    Operator = FilterOperator.GreaterThanOrEqual
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(3, rs.Count);
        Assert.IsTrue(rs.All(c => c.Age >= 30));
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
                    Name = nameof(Customer.Age),
                    Value = ages,
                    Operator = FilterOperator.In
                }
            }
        };

        // Act

        var rs = rq
            .Apply(Customers).Data
            .ToList();

        // Assert

        Assert.That(rs, Has.Count.EqualTo(2));

        Assert.That(rs.All(c => c.Age.HasValue && ages.Contains(c.Age.Value)), Is.True);
    }

    [Test]
    public void Should_Filter_By_In_Json()
    {
        // Arrange

        var ages = new[] { 25, 40 };

        var json = JsonDocument.Parse("[ 25, 40 ]")
            .RootElement;

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Age),
                    Value = json,
                    Operator = FilterOperator.In
                }
            }
        };

        // Act

        var rs = rq
            .Apply(Customers).Data
            .ToList();

        // Assert

        Assert.That(rs, Has.Count.EqualTo(2));

        Assert.That(rs.All(c => c.Age.HasValue && ages.Contains(c.Age.Value)), Is.True);
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
                    Name = nameof(Customer.Age),
                    Value = ages,
                    Operator = FilterOperator.In
                }
            }
        };

        // Act

        var rs = rq
            .Apply(Customers).Data
            .ToList();

        // Assert

        Assert.That(rs, Has.Count.EqualTo(2));

        Assert.That(rs.All(c => c.Age.HasValue && ages.Contains(c.Age.Value)), Is.True);
    }

    [Test]
    public void Should_Filter_By_In_String()
    {
        // Arrange

        var names = new[] { "John Doe", "Jane Smith" };

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Name),
                    Value = names,
                    Operator = FilterOperator.In
                }
            }
        };

        // Act

        var rs = rq
            .Apply(Customers).Data
            .ToList();

        // Assert

        Assert.That(rs, Has.Count.EqualTo(2));

        Assert.That(rs.All(c => names.Contains(c.Name)), Is.True);
    }

    [Test]
    public void Should_Filter_By_Inequality()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Age),
                    Value = 30,
                    Operator = FilterOperator.NotEquals
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(2, rs.Count);
        Assert.IsTrue(rs.All(c => c.Age != 30));
    }

    [Test]
    public void Should_Filter_By_Inequality_BirthDate()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.BirthDate),
                    Value = "1986-12-02T00:00:00",
                    Operator = FilterOperator.NotEquals
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(3, rs.Count);
        Assert.IsTrue(rs.All(c => c.BirthDate != new DateTime(1986, 12, 2)));
    }

    [Test]
    public void Should_Filter_By_Inequality_Json()
    {
        // Arrange

        var json = JsonDocument.Parse("30")
            .RootElement;

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Age),
                    Value = json,
                    Operator = FilterOperator.NotEquals
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(2, rs.Count);
        Assert.IsTrue(rs.All(c => c.Age != 30));
    }

    [Test]
    public void Should_Filter_By_LessThan()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Age),
                    Value = 30,
                    Operator = FilterOperator.LessThan
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(1, rs.Count);
        Assert.IsTrue(rs.All(c => c.Age < 30));
    }

    [Test]
    public void Should_Filter_By_LessThanOrEqual()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Age),
                    Value = 30,
                    Operator = FilterOperator.LessThanOrEqual
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(3, rs.Count);
        Assert.IsTrue(rs.All(c => c.Age <= 30));
    }

    [Test]
    public void Should_Filter_By_NotContains()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Address),
                    Value = "St",
                    Operator = FilterOperator.NotContains
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(0, rs.Count);

        Assert.IsFalse(rs.Any(c => c.Address.Contains("St")));
    }

    [Test]
    public void Should_Filter_By_NotContains_Json()
    {
        // Arrange

        var json = JsonDocument.Parse("\"St\"")
            .RootElement;

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Address),
                    Value = json,
                    Operator = FilterOperator.NotContains
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(0, rs.Count);

        Assert.IsFalse(rs.Any(c => c.Address.Contains("St")));
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
                    Name = nameof(Customer.Age),
                    Value = ages,
                    Operator = FilterOperator.NotIn
                }
            }
        };

        // Act

        var rs = rq
            .Apply(Customers).Data
            .ToList();

        // Assert

        Assert.That(rs, Has.Count.EqualTo(2));

        Assert.That(rs.All(c => !c.Age.HasValue || !ages.Contains(c.Age.Value)), Is.True);
    }

    [Test]
    public void Should_Filter_By_NotIn_Json()
    {
        // Arrange

        var ages = new[] { 25, 40 };

        var json = JsonDocument.Parse("[ 25, 40 ]")
            .RootElement;

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Age),
                    Value = json,
                    Operator = FilterOperator.NotIn
                }
            }
        };

        // Act

        var rs = rq
            .Apply(Customers).Data
            .ToList();

        // Assert

        Assert.That(rs, Has.Count.EqualTo(2));

        Assert.That(rs.All(c => !c.Age.HasValue || !ages.Contains(c.Age.Value)), Is.True);
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
                    Name = nameof(Customer.Age),
                    Value = ages,
                    Operator = FilterOperator.NotIn
                }
            }
        };

        // Act

        var rs = rq
            .Apply(Customers).Data
            .ToList();

        // Assert

        Assert.That(rs, Has.Count.EqualTo(2));

        Assert.That(rs.All(c => !c.Age.HasValue || !ages.Contains(c.Age.Value)), Is.True);
    }

    [Test]
    public void Should_Filter_By_NotIn_String()
    {
        // Arrange

        var names = new[] { "John Doe", "Jane Smith" };

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Name),
                    Value = names,
                    Operator = FilterOperator.NotIn
                }
            }
        };

        // Act

        var rs = rq
            .Apply(Customers).Data
            .ToList();

        // Assert

        Assert.That(rs, Has.Count.EqualTo(2));

        Assert.That(!rs.All(c => names.Contains(c.Name)), Is.True);
    }

    [Test]
    public void Should_Filter_By_StartsWith()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Filters = new()
            {
                new()
                {
                    Name = nameof(Customer.Name),
                    Value = "John",
                    Operator = FilterOperator.StartsWith
                }
            }
        };

        // Act

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(1, rs.Count);
        Assert.IsTrue(rs.All(c => c.Name.StartsWith("John")));
    }

    [Test]
    public void Should_Filter_Using_CustomExpression()
    {
        // Arrange

        static bool func(Customer c) => c.Age > 30;

        Expression<Func<Customer, bool>> exp = c => c.Age > 30;

        FilterRequest rq = new()
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

        var rs = rq.Apply(Customers).Data.ToList();

        // Assert

        Assert.AreEqual(1, rs.Count);
        Assert.IsTrue(rs.All(func));
    }

    [Test]
    public void Should_Get_TotalCount_Without_Pagination()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Count = true
        };

        // Act

        var count = rq.Apply(Customers).TotalCount;

        // Assert

        Assert.AreEqual(4, count);
    }

    [Test]
    public void Should_Paginate_Results()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Page = 2,
            PageSize = 2,
            Count = true
        };

        // Act

        var rs = rq.Apply(Customers);

        // Assert

        Assert.AreEqual(2, rs.Data.Count);
        Assert.AreEqual(2, rs.Page);
        Assert.AreEqual(2, rs.PageSize);
        Assert.AreEqual(4, rs.TotalCount);
    }

    [Test]
    public void Should_Select_Properties()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Select = new() {
                nameof(Customer.Id),
                nameof(Customer.Name)
            }
        };

        // Act

        var rs = rq.Apply(Customers)
            .Data
            .ToList();

        // Assert

        Assert.AreEqual(4, rs.Count);

        Assert.IsTrue(rs.All(c => c.Age == null && c.Address == null));
    }

    [Test]
    public void Should_Sort_By_Name_Ascending()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Sort = new SortExpression
            {
                Name = nameof(Customer.Name),
                Direction = SortDirection.Ascending
            }
        };

        // Act

        var sortedCustomers = rq.Apply(Customers).Data.ToList();

        var expectedOrder = Customers.OrderBy(c => c.Name).ToList();

        // Assert

        Assert.AreEqual(expectedOrder, sortedCustomers);
    }

    [Test]
    public void Should_Sort_By_Name_Descending()
    {
        // Arrange

        FilterRequest rq = new()
        {
            Sort = new SortExpression
            {
                Name = nameof(Customer.Name),
                Direction = SortDirection.Descending
            }
        };

        // Act

        var sortedCustomers = rq.Apply(Customers).Data.ToList();

        var expectedOrder = Customers.OrderByDescending(c => c.Name).ToList();

        // Assert

        Assert.AreEqual(expectedOrder, sortedCustomers);
    }

    public class Customer
    {
        public string Address { get; set; }
        public int? Age { get; set; }
        public DateTime? BirthDate { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}