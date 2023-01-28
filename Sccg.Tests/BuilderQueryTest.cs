using System.Reflection;
using Sccg.Core;

namespace Sccg.Tests;

public class BuilderQueryTest
{
    private readonly Builder _builder;
    private readonly BuilderQuery _query;

    public BuilderQueryTest()
    {
        _builder = new Builder();
        _query = GetPrivateField<BuilderQuery>(_builder, "_query");
    }

    [Fact]
    public void BuilderUse()
    {
        var source1 = MockGen.Source("source1", 10, () => new[] { new Item("item1", "prop1") }, _ => { });
        var source2 = MockGen.Source("source2", 10, () => new[] { new Item("item2", "prop2") }, _ => { });
        var formatter1 = MockGen.Formatter("formatter1", 10, (_, _) => new Content("content1"));
        var formatter2 = MockGen.Formatter("formatter2", 10, (_, _) => new Content("content2"));
        var formatter3 = MockGen.Formatter("formatter3", 10, (_, _) => new Content("content3"));
        var writer1 = MockGen.Writer("writer1", 10, (_, _) => { });

        _builder.Use(new[] { source1, source2 });
        _query.GetSources<ISource>().Should().BeEquivalentTo(new[] { source1, source2 });
        _query.GetFormatters<IFormatter>(allowEmptyReturn: true).Should().BeEmpty();
        _query.GetWriters<IWriter>(allowEmptyReturn: true).Should().BeEmpty();

        _builder.Use(new[] { formatter1, formatter2, formatter3 });
        _query.GetSources<ISource>().Should().BeEquivalentTo(new[] { source1, source2 });
        _query.GetFormatters<IFormatter>().Should().BeEquivalentTo(new[] { formatter1, formatter2, formatter3 });
        _query.GetWriters<IWriter>(allowEmptyReturn: true).Should().BeEmpty();

        _builder.Use(new[] { writer1 });
        _query.GetSources<ISource>().Should().BeEquivalentTo(new[] { source1, source2 });
        _query.GetFormatters<IFormatter>().Should().BeEquivalentTo(new[] { formatter1, formatter2, formatter3 });
        _query.GetWriters<IWriter>().Should().BeEquivalentTo(new[] { writer1 });
    }

    [Fact]
    public void DuplicateUse()
    {
        var source1 = MockGen.Source("source", 10, () => new[] { new Item("item1", "prop1") }, _ => { });
        var source2 = MockGen.Source("source", 10, () => new[] { new Item("item2", "prop2") }, _ => { });

        _builder.Use(source1);
        Assert.Throws<ArgumentException>(() => _builder.Use(source2));
        Assert.Throws<ArgumentException>(() => _query.RegisterSource(source2));
    }

    [Fact]
    public void RegisterFromSource()
    {
        var addSource1 = MockGen.Source("add_source1", 10, () => new[] { new Item("item1", "prop1") }, _ => { });
        var addSource2 = MockGen.Source("add_source2", 5, () => new[] { new Item("item2", "prop2") }, _ => { });
        var addFormatter = MockGen.Formatter("add_formatter", 10, (_, _) => new Content("content"));
        var addWriter = MockGen.Writer("add_writer", 10, (_, _) => { });
        var source = MockGen.Source("first_source", 10, () => new[] { new Item("item", "prop") }, query =>
        {
            query.RegisterSource(addSource1);
            query.RegisterSource(addSource2);
            query.RegisterFormatter(addFormatter);
            query.RegisterWriter(addWriter);
        });
        _builder.Use(source);
        _builder.Build();

        var sourceItems = _query.GetSourceItems<ISourceItem>();
        _query.GetSources<ISource>().Should().BeEquivalentTo(new[] { source, addSource1, addSource2 });
        sourceItems.Should().BeEquivalentTo(new[]
        {
            new Item("item", "prop"),
            new Item("item2", "prop2"),
            new Item("item1", "prop1"),
        }, opt => opt.WithStrictOrderingFor(x => x));

        var contents = _query.GetContents<IContent>();
        _query.GetFormatters<IFormatter>().Should().BeEquivalentTo(new[] { addFormatter });
        contents.Should().BeEquivalentTo(new[] { new Content("content") });

        _query.GetWriters<IWriter>().Should().BeEquivalentTo(new[] { addWriter });
    }

    [Fact]
    public void RegisterFromFormatter()
    {
        var addFormatter1 = MockGen.Formatter("add_formatter1", 10, (_, _) => new Content("content1"));
        var addFormatter2 = MockGen.Formatter("add_formatter2", 5, (_, _) => new Content("content2"));
        var addWriter = MockGen.Writer("add_writer", 10, (_, _) => { });
        var formatter = MockGen.Formatter("first_formatter", 10, (_, query) =>
        {
            query.RegisterFormatter(addFormatter1);
            query.RegisterFormatter(addFormatter2);
            query.RegisterWriter(addWriter);
            return new Content("content");
        });
        _builder.Use(formatter);
        _builder.Build();

        var contents = _query.GetContents<IContent>();
        _query.GetFormatters<IFormatter>().Should().BeEquivalentTo(new[] { formatter, addFormatter1, addFormatter2 });
        contents.Should().BeEquivalentTo(new[]
        {
            new Content("content"),
            new Content("content2"),
            new Content("content1"),
        }, opt => opt.WithStrictOrderingFor(x => x));

        _query.GetWriters<IWriter>().Should().BeEquivalentTo(new[] { addWriter });
    }

    [Fact]
    public void RegisterFromWriter()
    {
        var w = new List<string>();
        var addWriter1 = MockGen.Writer("add_writer1", 10, (_, _) => { w.Add("w1"); });
        var addWriter2 = MockGen.Writer("add_writer2", 5, (_, _) => { w.Add("w2"); });
        var writer = MockGen.Writer("first_writer", 10, (_, query) =>
        {
            query.RegisterWriter(addWriter1);
            query.RegisterWriter(addWriter2);
            w.Add("w");
        });
        _builder.Use(writer);
        _builder.Build();

        _query.GetWriters<IWriter>().Should().BeEquivalentTo(new[] { writer, addWriter1, addWriter2 });
        w.Should().BeEquivalentTo(new[]
        {
            "w", "w2", "w1"
        }, opt => opt.WithStrictOrderingFor(x => x));
    }

    [Fact]
    public void InvalidRegisterFromFormatter()
    {
        var source = MockGen.Source("source", 10, () => new[] { new Item("item", "prop") }, _ => { });
        var formatter = MockGen.Formatter("formatter", 10, (_, query) =>
        {
            Assert.Throws<InvalidOperationException>(() => query.RegisterSource(source));
            return new Content("content");
        });
        _builder.Use(formatter);
        _builder.Build();
    }

    [Fact]
    public void InvalidRegisterFromWriter()
    {
        var source = MockGen.Source("source", 10, () => new[] { new Item("item", "prop") }, _ => { });
        var writer = MockGen.Writer("writer", 10, (_, query) =>
        {
            Assert.Throws<InvalidOperationException>(() => query.RegisterSource(source));
        });
        _builder.Use(writer);
        _builder.Build();
    }

    [Fact]
    public void CheckFilter()
    {
        var source = MockGen.Source("source", 10, () => new ISourceItem[]
        {
            new DummyItem(),
            new Item("item", "prop"),
            new DummyItem(),
            new Item("item1", "prop1"),
        }, _ => { });
        _builder.Use(source);
        _builder.Build();

        _query.GetSourceItems<Item>().Should().BeEquivalentTo(new[]
        {
            new Item("item", "prop"),
            new Item("item1", "prop1")
        });
    }

    private static T GetPrivateField<T>(object obj, string name)
    {
        var type = obj.GetType();
        var field = type.GetField(name,
            BindingFlags.Public | BindingFlags.NonPublic | // All access levels
            BindingFlags.Static | BindingFlags.Instance | // No consideration of static/non-static
            BindingFlags.GetField
        ) ?? throw new Exception($"Failed to get private fields: {name} in {type}.");
        return field.GetValue(obj) is T ret ? ret : throw new Exception($"Field value is null: {name} in {type}.");
    }
}

file class Item : ISourceItem
{
    public string Name { get; }
    public string Prop { get; }

    public Item(string name, string prop)
    {
        Name = name;
        Prop = prop;
    }
}

file class DummyItem : ISourceItem
{
}

file class Content : IContent
{
    public string Text { get; }

    public Content(string text)
    {
        Text = text;
    }
}

file static class MockGen
{
    public static ISource Source(
        string name,
        int priority,
        Func<IEnumerable<ISourceItem>> collectItems,
        Action<BuilderQuery> custom)
    {
        var mock = new Mock<ISource>();
        mock.SetupGet(x => x.Name).Returns(name);
        mock.SetupGet(x => x.Priority).Returns(priority);
        mock.Setup(x => x.CollectItems()).Returns(Enumerable.OfType<ISourceItem>(collectItems()));
        mock.Setup(x => x.Custom(It.IsAny<BuilderQuery>())).Callback(custom);
        return mock.Object;
    }

    public static IFormatter Formatter(
        string name,
        int priority,
        Func<IEnumerable<ISourceItem>, BuilderQuery, IContent> format)
    {
        var mock = new Mock<IFormatter>();
        mock.SetupGet(x => x.Name).Returns(name);
        mock.SetupGet(x => x.Priority).Returns(priority);
        mock.Setup(x => x.Format(It.IsAny<IEnumerable<ISourceItem>>(), It.IsAny<BuilderQuery>()))
            .Returns(format);
        return mock.Object;
    }

    public static IWriter Writer(
        string name,
        int priority,
        Action<IEnumerable<IContent>, BuilderQuery> write)
    {
        var mock = new Mock<IWriter>();
        mock.SetupGet(x => x.Name).Returns(name);
        mock.SetupGet(x => x.Priority).Returns(priority);
        mock.Setup(x => x.Write(It.IsAny<IEnumerable<IContent>>(), It.IsAny<BuilderQuery>())).Callback(write);
        return mock.Object;
    }
}