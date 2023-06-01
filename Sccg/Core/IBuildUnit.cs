namespace Sccg.Core;

public interface IBuildUnit
{
    /// <summary>
    /// Gets the <see cref="IBuildUnit"/> name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the order in which the <see cref="IBuildUnit"/> is applied.
    /// The lower the number, the earlier the <see cref="IBuildUnit"/> is applied.
    /// </summary>
    public int Priority { get; }
}