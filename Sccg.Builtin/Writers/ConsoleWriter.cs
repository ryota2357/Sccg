using System;
using System.Collections.Generic;
using Sccg.Core;

namespace Sccg.Builtin.Writers;

/// <summary>
/// A writer that writes to the console.
/// </summary>
public class ConsoleWriter : IWriter
{
    /// <inheritdoc />
    public string Name => "Console";

    /// <inheritdoc />
    public int Priority => 10;

    /// <inheritdoc />
    public void Write(IEnumerable<IContent> contents, BuilderQuery _)
    {
        foreach (var content in contents)
        {
            Console.WriteLine(content.ToString());
        }
    }
}