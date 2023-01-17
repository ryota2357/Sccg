using System.Linq;
using System.Collections.Generic;

namespace Sccg.Core;

public abstract class Source<TGroup, TItem> : ISource
{
    public abstract string Name { get; }

    public abstract void Custom();

    /// <inheritdoc cref="ISource.CollectItems"/>
    protected abstract IEnumerable<TItem> CollectItems();

    /// <summary>
    /// TODO: doc
    /// </summary>
    /// <param name="group"></param>
    /// <param name="style"></param>
    protected abstract void Set(TGroup group, Style style);

    /// <summary>
    /// TODO: doc
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    protected abstract void Link(TGroup from, TGroup to);

    public virtual int Priority => 0;

    /// <summary>
    /// TODO: doc
    /// </summary>
    /// <param name="group"></param>
    /// <param name="fg"></param>
    /// <param name="bg"></param>
    /// <param name="sp"></param>
    /// <param name="none"></param>
    /// <param name="bold"></param>
    /// <param name="italic"></param>
    /// <param name="strikethrough"></param>
    /// <param name="underline"></param>
    /// <param name="underlineWaved"></param>
    /// <param name="underlineDotted"></param>
    /// <param name="underlineDashed"></param>
    /// <param name="underlineDouble"></param>
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

    IEnumerable<ISourceItem> ISource.CollectItems()
    {
        return CollectItems().OfType<ISourceItem>();
    }
}