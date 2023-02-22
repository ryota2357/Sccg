using System;
using System.Collections.Generic;
using Sccg.Core;

namespace Sccg.Builtin.Writers;

public class ConsoleWriter : IWriter
{
    public string Name => "Console";

    public int Priority => 10;

    public void Write(IEnumerable<IContent> contents, BuilderQuery _)
    {
        foreach (var content in contents)
        {
            Console.WriteLine(content.ToString());
        }
    }
}