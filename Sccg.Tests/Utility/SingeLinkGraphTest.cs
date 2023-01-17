using Sccg.Utility;

namespace Sccg.Tests.Utility;

public class SingeLinkGraphTest
{
    private readonly SingeLinkGraph _graph = new();

    [Fact]
    public void Construct()
    {
        var a = new SingeLinkGraph();
        a.GetLink(0).Should().BeNull();
        a.TryGetLink(0, out var ato).Should().BeFalse();
        ato.Should().BeNull();
        a.TopologicalOrderList().Should().BeEmpty();

        var b = new SingeLinkGraph(7);
        b.GetLink(0).Should().BeNull();
        b.TryGetLink(0, out var bto).Should().BeFalse();
        bto.Should().BeNull();
        b.TopologicalOrderList().Should().BeEmpty();

        Assert.Throws<ArgumentOutOfRangeException>(() => new SingeLinkGraph(-1));
    }

    [Fact]
    public void LinkCheck()
    {
        CreateLinks(new[] { (1, 2), (2, 3), (3, 1) });

        foreach (var (a, b) in new (int, int?)[] { (2, 3), (1, 2), (3, 1), (4, null) })
        {
            var next = _graph.GetLink(a);
            next.Should().Be(b);
        }

        _graph.CreateLink(4, 1);
        _graph.GetLink(4).Should().Be(1);
    }

    // Cannot pass (int, int)[] ... by [InlineData] attribute

    [Fact]
    public void TopologicalSort1()
    {
        // 3 <- 2 <- 1
        CreateLinks(new[] { (1, 2), (2, 3) });
        CheckTopological();
    }

    [Fact]
    public void TopologicalSort2()
    {
        // 3 <- 2 <- 1
        //   <- 4
        CreateLinks(new[] { (1, 2), (2, 3), (4, 3) });
        CheckTopological();
    }

    [Fact]
    public void TopologicalSort3()
    {
        // 3 <- 2 <- 1 | 5 <- 6 <- 20 | 8 <- 9
        //   <- 4          <- 15
        CreateLinks(new[] { (1, 2), (2, 3), (4, 3), (6, 5), (20, 6), (15, 5), (9, 8) });
        CheckTopological();
    }

    [Fact]
    public void TopologicalSort4()
    {
        CreateLinks(new[] { (1, 2), (2, 1) });
        Assert.Throws<InvalidOperationException>(() => _graph.TopologicalOrderList());
    }

    [Fact]
    public void Overwrite()
    {
        CreateLinks(new[] { (1, 2), (1, 3) });
        _graph.GetLink(1).Should().Be(2);
        _graph.CreateLink(1, 3, overwrite: true);
        _graph.GetLink(1).Should().Be(3);
    }

    [Fact]
    public void Exception()
    {
        Assert.Throws<ArgumentException>(() => _graph.CreateLink(-1, 1));
        Assert.Throws<ArgumentException>(() => _graph.CreateLink(1, -1));
        Assert.Throws<ArgumentException>(() => _graph.GetLink(-1));
    }

    private void CreateLinks(IEnumerable<(int, int)> data)
    {
        foreach (var (a, b) in data)
        {
            _graph.CreateLink(a, b);
        }
    }

    private void CheckTopological()
    {
        var sorted = _graph.TopologicalOrderList().ToArray();
        var seen = new HashSet<int>();
        foreach (var node in sorted)
        {
            var next = _graph.GetLink(node);
            if (next is not null)
            {
                seen.Contains(next.Value).Should().BeTrue();
            }

            seen.Add(node);
        }
    }
}