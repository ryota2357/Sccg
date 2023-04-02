using Sccg.Utility;

namespace Sccg.Tests.Utility;

public class ObjectStoreTest
{
    private readonly ObjectStore _store;

    public ObjectStoreTest()
    {
        _store = new ObjectStore();
    }

    [Fact]
    public void CountForStruct()
    {
        var a = new TestStruct(7);
        _store.Save(a);
        _store.Count.Should().Be(1);

        _store.Save(a);
        _store.Count.Should().Be(1);

        var b = new TestStruct(7);
        _store.Save(b);
        _store.Count.Should().Be(1);
    }

    [Fact]
    public void CountForClass()
    {
        var a = new TestClass(7);
        _store.Save(a);
        _store.Count.Should().Be(1);

        _store.Save(a);
        _store.Count.Should().Be(1);

        var b = new TestClass(13);
        _store.Save(b);
        _store.Count.Should().Be(2);
    }

    [Fact]
    public void Id()
    {
        var a = 10;
        var aId = _store.Save(a);
        aId.Should().Be(0);

        var b = 20;
        var bId = _store.Save(b);
        bId.Should().Be(1);

        var c = new TestClass(3);
        var cId = _store.Save(c);
        cId.Should().Be(2);

        var d = 10;
        var dId = _store.Save(d);
        dId.Should().Be(aId);

        var e = new TestClass(7);
        var eId = _store.Save(e);
        eId.Should().NotBe(cId);
    }

    [Fact]
    public void LoadForStruct()
    {
        var a = new TestStruct(13);
        var b = new TestStruct(17);
        var aId = _store.Save(a);
        var _ = _store.Save(b);

        var c = _store.Load<TestStruct>(aId);
        c.Should().Be(a);
        c.Should().NotBe(b);
        c.Id.Should().Be(13);

        var cData = _store.Load(aId);
        cData.type.Should().Be(typeof(TestStruct));
        cData.data.Should().Be(a);
    }

    [Fact]
    public void LoadForClass()
    {
        var a = new TestClass(13);
        var b = new TestClass(17);
        var aId = _store.Save(a);
        var _ = _store.Save(b);

        var c = _store.Load<TestClass>(aId);
        c.Should().Be(a);
        c.Should().NotBe(b);
        c.Id.Should().Be(13);

        var cData = _store.Load(aId);
        cData.type.Should().Be(typeof(TestClass));
        cData.data.Should().Be(a);
    }

    [Fact]
    public void LoadException()
    {
        var a = new TestClass(3);
        var aId = _store.Save(a);
        Assert.Throws<InvalidCastException>(() => _store.Load<TestStruct>(aId));
        Assert.Throws<ArgumentOutOfRangeException>(() => _store.Load(aId + 1));
    }

    [Fact]
    public void TryLoad()
    {
        var a = new TestClass(3);
        var aId = _store.Save(a);
        _store.TryLoad<TestClass>(aId, out _).Should().BeTrue();
        _store.TryLoad<TestClass>(aId + 1, out _).Should().BeFalse();
        _store.TryLoad<TestStruct>(aId, out _).Should().BeFalse();
    }
}

file struct TestStruct
{
    public int Id;

    public TestStruct(int id)
    {
        Id = id;
    }
}

file class TestClass
{
    public readonly int Id;

    public TestClass(int id)
    {
        Id = id;
    }
}