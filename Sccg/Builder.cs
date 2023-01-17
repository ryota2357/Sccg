using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Sccg.Core;

namespace Sccg;

public class Builder
{
    private readonly List<IFormatter> _formatters = new();
    private readonly List<ISource> _sources = new();
    private readonly List<IContentWriter> _writers = new();

    public Metadata Metadata { get; init; } = Metadata.Empty;

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
        _sources.Sort((a, b) => a.Priority - b.Priority);
        _formatters.Sort((a, b) => a.Priority - b.Priority);
        _writers.Sort((a, b) => a.Priority - b.Priority);

        Log.Info("Build start");

        var sourceItems = CollectItems();
        Log.Info($"Collect {sourceItems.Count} items from {_sources.Count} sources.");

        var contents = FormatItems(sourceItems);
        Log.Info($"Format {contents.Count} content from {_formatters.Count} formatters.");

        foreach (var writer in _writers)
        {
            writer.Write(contents);
        }

        Log.Info("Build completed.");
    }

    public void Use<T>(T instance) where T : class
    {
        if (instance is IMetadataUser metadataUser)
        {
            metadataUser.SetMetadata(Metadata);
        }

        switch (instance)
        {
            case IFormatter formatter:
                _formatters.Add(formatter);
                break;
            case ISource source:
                _sources.Add(source);
                break;
            case IContentWriter writer:
                _writers.Add(writer);
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

    private List<ISourceItem> CollectItems()
    {
        var ret = new List<ISourceItem>();

        foreach (var source in _sources)
        {
            IEnumerable<ISourceItem> items;
            try
            {
                source.Custom();
                items = source.CollectItems();
            }
            catch (Exception e)
            {
                throw new Exception($"Source {source.Name} failed.", e);
            }
            ret.AddRange(items);
        }

        return ret;
    }

    private List<IContent> FormatItems(IReadOnlyCollection<ISourceItem> sourceItems)
    {
        var ret = new List<IContent>();
        foreach (var formatter in _formatters)
        {
            IContent content;
            try
            {
                content = formatter.Format(sourceItems);
            }
            catch (Exception e)
            {
                throw new Exception($"Formatter {formatter.Name} failed.", e);
            }
            ret.Add(content);
        }

        return ret;
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