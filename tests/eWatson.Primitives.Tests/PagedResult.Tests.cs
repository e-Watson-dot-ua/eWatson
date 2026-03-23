using eWatson.Abstractions.Specifications.Pagination;
using eWatson.Primitives.Results;
using FluentAssertions;

namespace eWatson.Primitives.Tests;

public sealed class PagedResultTests
{
    [Fact]
    public void Create_WithItems_HasCorrectPageInfo()
    {
        var items = new[] { "a", "b", "c" };
        var paging = new Paging(1, 3);

        var result = PagedResult<string>.Create(items, paging, 10);

        result.Items.Should().HaveCount(3);
        result.PageInfo.TotalItems.Should().Be(10);
        result.PageInfo.TotalPages.Should().Be(4);
        result.PageInfo.HasNext.Should().BeTrue();
        result.PageInfo.HasPrevious.Should().BeFalse();
    }

    [Fact]
    public void Empty_HasNoItems()
    {
        var paging = new Paging(1, 10);

        var result = PagedResult<string>.Empty(paging);

        result.Items.Should().BeEmpty();
        result.PageInfo.TotalItems.Should().Be(0);
    }

    [Fact]
    public void Map_TransformsItems()
    {
        var items = new[] { 1, 2, 3 };
        var paging = new Paging(1, 10);
        var result = PagedResult<int>.Create(items, paging, 3);

        var mapped = result.Map(i => i.ToString(System.Globalization.CultureInfo.InvariantCulture));

        mapped.Items.Should().BeEquivalentTo(["1", "2", "3"]);
        mapped.PageInfo.Should().Be(result.PageInfo);
    }

    [Fact]
    public void Paging_InvalidPage_Throws()
    {
        var act = () => new Paging(0, 10);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Paging_InvalidSize_Throws()
    {
        var act = () => new Paging(1, 0);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Paging_Skip_CalculatesCorrectly()
    {
        var paging = new Paging(3, 10);

        paging.Skip.Should().Be(20);
    }
}
