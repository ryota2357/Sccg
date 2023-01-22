using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Sccg.Core;

namespace Sccg;

/// <summary>
/// The builder of color configuration.
/// </summary>
public class Builder
{
    private readonly BuilderQuery _query = new();

    /// <summary>
    /// The metadata for all process.
    /// </summary>
    public Metadata Metadata
    {
        init => _query.RegisterMetadata(value);
    }

    /// <summary>
    /// The log level for <see cref="Build"/>.
    /// </summary>
    public LogLevel LogLevel
    {
        init => LoggerConfig.Shared.Level = value;
    }

    /// <summary>
    /// The log file for <see cref="Build"/>.
    /// </summary>
    public string? LogFile
    {
        init => LoggerConfig.Shared.File = value;
    }

    /// <summary>
    /// Starts to build the color configuration.
    /// </summary>
    public void Build()
    {
        Log.Info("Build start");

        var contents = _query.GetContents<IContent>();
        foreach (var writer in _query.GetWriters<IWriter>())
        {
            writer.Write(contents, _query);
        }

        Log.Info("Build completed.");
    }

    /// <summary>
    /// Registers the Source, Formatter, Writer instance.
    /// </summary>
    /// <param name="instance">The instance of Source, Formatter, Writer.</param>
    /// <typeparam name="T"><see cref="ISource"/>, <see cref="IFormatter"/>, <see cref="IWriter"/></typeparam>
    public void Use<T>(T instance) where T : class
    {
        switch (instance)
        {
            case ISource source:
                _query.RegisterSource(source);
                break;
            case IFormatter formatter:
                _query.RegisterFormatter(formatter);
                break;
            case IWriter writer:
                _query.RegisterWriter(writer);
                break;
            default:
                throw new ArgumentException("Invalid type", nameof(T));
        }
    }

    /// <summary>
    /// Registers multiple the Source, Formatter, Writer.
    /// </summary>
    /// <param name="instances">The instances of Source, Formatter, Writer.</param>
    /// <typeparam name="T"><see cref="ISource"/>, <see cref="IFormatter"/>, <see cref="IWriter"/></typeparam>
    public void Use<T>(IEnumerable<T> instances) where T : class
    {
        foreach (var instance in instances)
        {
            Use(instance);
        }
    }

    /// <summary>
    /// Registers the Source, Formatter, Writer.
    /// </summary>
    /// <param name="args">The argument of its constructor.</param>
    /// <typeparam name="T"><see cref="ISource"/>, <see cref="IFormatter"/>, <see cref="IWriter"/></typeparam>
    public void Use<T>(params object[] args) where T : class
    {
        var instance = CreateInstance<T>(args);
        Use(instance);
    }

    private static T CreateInstance<T>(params object[] args) where T : class
    {
        object? instance;
        try
        {
            instance = Activator.CreateInstance(
                type: typeof(T),
                bindingAttr: BindingFlags.CreateInstance
                             | BindingFlags.Public
                             | BindingFlags.Instance
                             | BindingFlags.OptionalParamBinding,
                binder: null,
                args: args,
                culture: null
            );
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Failed to create instance of {typeof(T).Name}", e);
        }

        if (instance is T t)
        {
            return t;
        }

        throw new ConstraintException($"Created instance is null: {typeof(T).Name} with args: {args}");
    }
}