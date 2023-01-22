using System;
using System.Linq;
using System.Collections.Generic;

namespace Sccg.Core;

public abstract class Source<TGroup, TItem> : ISource
{
    /// <inheritdoc />
    public abstract string Name { get; }

    // TODO: doc
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

    // TODO: doc
    protected virtual void Custom()
    {
        throw new NotImplementedException("You must override Custom method.");
    }

    // TODO: doc
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
    private bool _custom;
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