using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Sccg.Core;

namespace Sccg;

/// <summary>
/// The builder of color configuration.
/// </summary>
public class Builder
{
    private readonly Container<ISource> _sources = new(ContainerType.Source);
    private readonly Container<IFormatter> _formatters = new(ContainerType.Formatter);
    private readonly Container<IWriter> _writers = new(ContainerType.Writer);
    private readonly Container<ISourceItemConverter> _sourceItemConverters = new(ContainerType.SourceItemConverter);
    private readonly Container<IContentConverter> _contentConverters = new(ContainerType.ContentConverter);
    private List<ISourceItem> _sourceItems = new();
    private List<IContent> _contents = new();
    private readonly BuilderQuery _query;
    private State _state = State.NotStarted;

    /// <summary>
    /// Initializes a new instance of the <see cref="Builder"/> class.
    /// </summary>
    public Builder()
    {
        _query = new BuilderQuery(this);
    }

    /// <summary>
    /// The metadata for all process.
    /// </summary>
    public Metadata Metadata { init; get; } = Metadata.Empty;

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
        _state = State.Started;
        Log.Info("Build start");

        _state = State.CollectingSourceItems;
        while (_sources.TryPop(out var source))
        {
            Log.Debug($"Collecting source items from {source.Name}({source.GetType().Name}).");
            try
            {
                source.Custom(_query);
                var items = source.CollectItems();
                _sourceItems.AddRange(items);
            }
            catch (Exception e)
            {
                throw new Exception($"Source {source.Name}({source.GetType().Name}) failed.", e);
            }
        }

        _state = State.ConvertingSourceItems;
        while (_sourceItemConverters.TryPop(out var converter))
        {
            Log.Debug($"Converting source items with {converter.Name}({converter.GetType().Name}).");
            try
            {
                _sourceItems = converter.Convert(_sourceItems, _query).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Source item converter {converter.Name}({converter.GetType().Name}) failed.", e);
            }
        }

        _state = State.FormattingSourceItems;
        while (_formatters.TryPop(out var formatter))
        {
            Log.Debug($"Formatting source items with {formatter.Name}({formatter.GetType().Name}).");
            try
            {
                var content = formatter.Format(_sourceItems.AsReadOnly(), _query);
                _contents.Add(content);
            }
            catch (Exception e)
            {
                throw new Exception($"Formatter {formatter.Name}({formatter.GetType().Name}) failed.", e);
            }
        }

        _state = State.ConvertingContents;
        while (_contentConverters.TryPop(out var converter))
        {
            Log.Debug($"Converting contents with {converter.Name}({converter.GetType().Name}).");
            try
            {
                _contents = converter.Convert(_contents, _query).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Content converter {converter.Name}({converter.GetType().Name}) failed.", e);
            }
        }

        _state = State.WritingContents;
        while (_writers.TryPop(out var writer))
        {
            Log.Debug($"Writing contents with {writer.Name}({writer.GetType().Name}).");
            try
            {
                writer.Write(_contents.AsReadOnly(), _query);
            }
            catch (Exception e)
            {
                throw new Exception($"Writer {writer.Name}({writer.GetType().Name}) failed.", e);
            }
        }

        _state = State.Completed;
        Log.Info("Build completed.");
    }

    /// <summary>
    /// Registers the Source, Formatter, Writer instance.
    /// </summary>
    /// <param name="instance">The instance of Source, Formatter, Writer.</param>
    /// <typeparam name="T"><see cref="ISource"/>, <see cref="IFormatter"/>, <see cref="IWriter"/></typeparam>
    /// <exception cref="InvalidOperationException">
    ///   <list type="bullet">
    ///     <item>Added source after collecting source items.</item>
    ///     <item>Added formatter after formatting source items.</item>
    ///     <item>Added writer after writing contents.</item>
    ///     <item>Added source item converter after converting source items.</item>
    ///     <item>Added content converter after converting content.</item>
    ///   </list>
    /// </exception>
    /// <exception cref="ArgumentException">Argument is not <see cref="ISource"/> or <see cref="IFormatter"/> or <see cref="IWriter"/></exception>
    public void Use<T>(T instance) where T : class
    {
        switch (instance)
        {
            case ISource source:
                if (_state > State.CollectingSourceItems)
                {
                    throw new InvalidOperationException("Cannot add source after collecting source items.");
                }
                _sources.Push(source, source.Priority, source.Name);
                break;
            case IFormatter formatter:
                if (_state > State.FormattingSourceItems)
                {
                    throw new InvalidOperationException("Cannot add formatter after formatting source items.");
                }
                _formatters.Push(formatter, formatter.Priority, formatter.Name);
                break;
            case IWriter writer:
                if (_state > State.WritingContents)
                {
                    throw new InvalidOperationException("Cannot add writer after writing contents.");
                }
                _writers.Push(writer, writer.Priority, writer.Name);
                break;
            case ISourceItemConverter sourceItemConverter:
                if (_state > State.ConvertingSourceItems)
                {
                    throw new InvalidOperationException(
                        "Cannot add source item converter after converting source items.");
                }
                _sourceItemConverters.Push(sourceItemConverter, sourceItemConverter.Priority, sourceItemConverter.Name);
                break;
            case IContentConverter contentConverter:
                if (_state > State.ConvertingContents)
                {
                    throw new InvalidOperationException("Cannot add content converter after converting content.");
                }
                _contentConverters.Push(contentConverter, contentConverter.Priority, contentConverter.Name);
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
    public void Use<T>(T[] instances) where T : class
    {
        // NOTE: The argument type is T[], not IEnumerable<T>.
        //       If the argument type is IEnumerable<T>, this method is not called, call Use<T>(T instance)
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

    internal IEnumerable<ISource> GetSources() => _sources.Items;

    internal IEnumerable<IFormatter> GetFormatters() => _formatters.Items;

    internal IEnumerable<IWriter> GetWriters() => _writers.Items;

    internal IEnumerable<ISourceItemConverter> GetSourceItemConverters() => _sourceItemConverters.Items;

    internal IEnumerable<IContentConverter> GetContentConverters() => _contentConverters.Items;

    internal IEnumerable<ISourceItem> GetSourceItems() => _sourceItems.AsReadOnly();

    internal IEnumerable<IContent> GetContents() => _contents.AsReadOnly();

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

    private enum State
    {
        NotStarted = 0,
        Started = 1,
        CollectingSourceItems = 2,
        ConvertingSourceItems = 3,
        FormattingSourceItems = 4,
        ConvertingContents = 5,
        WritingContents = 6,
        Completed = 7,
    }

    private enum ContainerType
    {
        Source,
        Formatter,
        Writer,
        SourceItemConverter,
        ContentConverter,
    }

    private sealed class Container<T>
    {
        private readonly List<T> _items = new();
        private readonly PriorityQueue<T, int> _queue = new();
        private readonly HashSet<string> _names = new();
        private readonly ContainerType _containerType;

        public ReadOnlyCollection<T> Items { get; }

        public Container(ContainerType containerType)
        {
            _containerType = containerType;
            Items = new ReadOnlyCollection<T>(_items);
        }

        public void Push(T item, int priority, string name)
        {
            if (_names.Contains(name))
            {
                throw new ArgumentException($"Duplicate {_containerType.ToString()}: {name}");
            }
            _items.Add(item);
            _queue.Enqueue(item, priority);
            _names.Add(name);
        }

        public bool TryPop([MaybeNullWhen(false)] out T item)
        {
            return _queue.TryDequeue(out item, out _);
        }
    }
}