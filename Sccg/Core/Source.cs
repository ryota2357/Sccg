using System;
using System.Linq;
using System.Collections.Generic;

namespace Sccg.Core;

/// <summary>
/// The base class of the <see cref="ISource"/>. This class provides some helper methods and standard implementation of <see cref="ISource"/>.
/// </summary>
public abstract class Source<TGroup, TItem> : ISource
    where TItem : ISourceItem
{
    /// <inheritdoc />
    public abstract string Name { get; }

    /// <summary>
    /// Collects source items from the source.
    /// </summary>
    /// <remarks>This method is called only once.</remarks>
    protected abstract IEnumerable<TItem> CollectItems();

    /// <summary>
    /// Sets style to group.
    /// </summary>
    /// <param name="group">A syntax/design group name.</param>
    /// <param name="style">A style.</param>
    protected abstract void Set(TGroup group, Style style);

    /// <summary>
    /// Links style `from` group to `to` group.
    /// </summary>
    /// <param name="from">A syntax/design group name.</param>
    /// <param name="to">A syntax/design group name.</param>
    protected abstract void Link(TGroup from, TGroup to);

    /// <inheritdoc />
    public virtual int Priority => 10;

    /// <summary>
    /// You can customize your theme here.
    /// </summary>
    /// <remarks>This method is called only once.</remarks>
    protected virtual void Custom()
    {
        throw new NotImplementedException("You must override Custom method.");
    }


    /// <inheritdoc cref="Custom()"/>
    protected virtual void Custom(BuilderQuery query)
    {
        Custom();
    }

    /// <summary>
    /// Sets style to group.
    /// </summary>
    /// <param name="group">A syntax/design group name.</param>
    /// <param name="fg">Foreground color.</param>
    /// <param name="bg">Background color.</param>
    /// <param name="sp">Special color.</param>
    /// <param name="none">Reset style decoration modifier.</param>
    /// <param name="bold">Bold font style.</param>
    /// <param name="italic">Italic font style.</param>
    /// <param name="strikethrough">Strikethrough font style.</param>
    /// <param name="underline">Underline font style.</param>
    /// <param name="underlineWaved">Underline Waved font style.</param>
    /// <param name="underlineDotted">Underline Dotted font style.</param>
    /// <param name="underlineDashed">Underline Dashed font style.</param>
    /// <param name="underlineDouble">Underline Double font style.</param>
    protected virtual void Set(
        TGroup group,
        Color? fg = null,
        Color? bg = null,
        Color? sp = null,
        bool none = false,
        bool bold = false,
        bool italic = false,
        bool strikethrough = false,
        bool underline = false,
        bool underlineWaved = false,
        bool underlineDotted = false,
        bool underlineDashed = false,
        bool underlineDouble = false)
    {
        Set(group, new Style(
            fg: fg,
            bg: bg,
            sp: sp,
            none: none,
            bold: bold,
            italic: italic,
            strikethrough: strikethrough,
            underline: underline,
            underlineWaved: underlineWaved,
            underlineDotted: underlineDotted,
            underlineDashed: underlineDashed,
            underlineDouble: underlineDouble
        ));
    }

    // This cache is necessary to respect Priority.
    private bool _custom = false;
    private IEnumerable<ISourceItem>? _collectItems = null;

    void ISource.Custom(BuilderQuery query)
    {
        if (_custom)
        {
            return;
        }

        Custom(query);
        _custom = true;
    }

    IEnumerable<ISourceItem> ISource.CollectItems()
    {
        return _collectItems ??= CollectItems().OfType<ISourceItem>();
    }
}

/// <summary>
/// The base class of the <see cref="ISource"/>. This class provides some helper methods and standard implementation of <see cref="ISource"/>.
/// </summary>
/// <remarks>Unlike <see cref="Source{TGroup,TItem}"/>, This class does not have Set(TGroup, Style), but has <see cref="Set"/></remarks>
public abstract class SourceColorOnly<TGroup, TItem> : ISource
{
    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc cref="Source{TGroup,TItem}.CollectItems()"/>
    protected abstract IEnumerable<TItem> CollectItems();

    /// <summary>
    /// Sets style to group.
    /// </summary>
    /// <param name="group">A syntax/design group name.</param>
    /// <param name="color">A color.</param>
    protected abstract void Set(TGroup group, Color color);

    /// <summary>
    /// Links style `from` group to `to` group.
    /// </summary>
    /// <param name="from">A syntax/design group name.</param>
    /// <param name="to">A syntax/design group name.</param>
    protected abstract void Link(TGroup from, TGroup to);

    /// <inheritdoc />
    public virtual int Priority => 10;

    /// <inheritdoc cref="Source{TGroup,TItem}.Custom()"/>
    protected virtual void Custom()
    {
        throw new NotImplementedException("You must override Custom method.");
    }

    /// <inheritdoc cref="Source{TGroup,TItem}.Custom(BuilderQuery)"/>
    protected virtual void Custom(BuilderQuery query)
    {
        Custom();
    }

    // This cache is necessary to respect Priority.
    private bool _custom = false;
    private IEnumerable<ISourceItem>? _collectItems = null;

    void ISource.Custom(BuilderQuery query)
    {
        if (_custom)
        {
            return;
        }

        Custom(query);
        _custom = true;
    }

    IEnumerable<ISourceItem> ISource.CollectItems()
    {
        return _collectItems ??= CollectItems().OfType<ISourceItem>();
    }
}