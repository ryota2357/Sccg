using System.Reflection;
using Sccg.Builtin.Formatters;
using Sccg.Builtin.Sources;

namespace Sccg.Tests.Builtin;

public class AlacrittyTest
{
    private readonly Builder _builder;
    private readonly TestWriter _writer;

    public AlacrittyTest()
    {
        _builder = new Builder
        {
            Metadata = Metadata.Default with
            {
                Author = "ryota2357",
                LastUpdated = DateTime.Parse("2023-01-23T01:23:45"),
                Footer = _ => new[] { "Built with Sccg" }
            }
        };
        _writer = new TestWriter();

        _builder.Use(_writer);
        _builder.Use<AlacrittyFormatter>();
    }

    [Fact]
    public void Test()
    {
        _builder.Use<Colors>();
        _builder.Build();

        _writer.Output.Should().HaveCount(1);
        var output = _writer.Output.First();
        output.Should().Be(
            """
            # Version:     1.0.0
            # Author:      ryota2357
            # Last change: 2023-01-23 Monday
            colors:
              bright:
                red: '#ff0000'
              normal:
                black: '#000000'
              primary:
                background: '#000000'
                foreground: '#ffffff'
            """
        );
    }

    [Fact]
    public void FormatterCreateYamlTest()
    {
        var data = new AlacrittyFormatter.Formattable[]
        {
            new(new[] { "aaa" }, "1"),
            new(new[] { "bbb", "aaa" }, "2"),
            new(new[] { "bbb", "bbb" }, "3"),
            new(new[] { "bbb", "ccc", "aaa" }, "4"),
            new(new[] { "ccc" }, "5"),
            new(new[] { "ddd", "aaa", "aaa", "aaa" }, "6"),
            new(new[] { "ddd", "aaa", "bbb" }, "7"),
            new(new[] { "ddd", "bbb" }, "8"),
        }.OrderBy(_ => Guid.NewGuid());
        var ret = Util.CallPrivateMethod<AlacrittyFormatter, string>(null, "CreateBody", data.ToList());
        ret.Should().Be(
            """
            aaa: 1
            bbb:
              aaa: 2
              bbb: 3
              ccc:
                aaa: 4
            ccc: 5
            ddd:
              aaa:
                aaa:
                  aaa: 6
                bbb: 7
              bbb: 8
            """);
    }

}

file class Colors : AlacrittyColorsSource
{
    protected override void Custom()
    {
        Set(Group.PrimaryForeground, "#ffffff");
        Set(Group.PrimaryBackground, "#000000");
        Set(Group.NormalBlack, "#000000");
        Set(Group.BrightRed, "#ff0000");
    }
}

file static class Util
{
    public static TReturn? CallPrivateMethod<TTarget, TReturn>(object? obj, string name, params object[] args)
    {
        var type = typeof(TTarget);
        var method = type.GetMethod(name,
            BindingFlags.Public | BindingFlags.NonPublic | // All access levels
            BindingFlags.Static | BindingFlags.Instance | // No consideration for static or instance
            BindingFlags.InvokeMethod,
            args.Select(a => a.GetType()).ToArray()
        );
        if (method is null)
        {
            throw new Exception($"Failed to call private method. {name} in {type} is not found.");
        }

        try
        {
            var ret = method.Invoke(obj, args);
            return ret is TReturn t ? t : default;
        }
        catch (Exception e)
        {
            throw e.InnerException ?? e;
        }
    }
}