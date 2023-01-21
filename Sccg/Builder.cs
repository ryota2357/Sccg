using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Sccg.Core;

namespace Sccg;

public class Builder
{
    private readonly BuilderQuery _query = new();

    public Metadata Metadata
    {
        init => _query.RegisterMetadata(value);
    }

    public Builder()
    {
    }

    public Builder(Metadata metadata)
    {
        _query.RegisterMetadata(metadata);
    }

    public LogLevel LogLevel
    {
        init => LoggerConfig.Shared.Level = value;
    }

    public string? LogFile
    {
        init => LoggerConfig.Shared.File = value;
    }

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

    public void Use<T>(T instance) where T : class
    {
        switch (instance)
        {
            case IFormatter formatter:
                _query.RegisterFormatter(formatter);
                break;
            case ISource source:
                _query.RegisterSource(source);
                break;
            case IWriter writer:
                _query.RegisterWriter(writer);
                break;
            default:
                throw new ArgumentException("Invalid type", nameof(T));
        }
    }

    public void Use<T>(IEnumerable<T> instances) where T : class
    {
        foreach (var instance in instances)
        {
            Use(instance);
        }
    }

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