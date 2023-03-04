using System;
using System.Collections.Generic;
using System.Linq;

namespace Sccg.Utility;

/// <summary>
/// Utility extensions.
/// </summary>
public static class UtilityExtensions
{
    /// <summary>
    /// Filter null values.
    /// </summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?>? source) where T : class
    {
        // NOTE: If use OfType<T>, Rider shows warning.
        return source == null ? Enumerable.Empty<T>() : source.Where(x => x is not null).Select(x => x!);
    }

    /// <inheritdoc cref="WhereNotNull{T}(System.Collections.Generic.IEnumerable{T?}?)"/>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?>? source) where T : struct
    {
        return source == null ? Enumerable.Empty<T>() : source.Where(x => x.HasValue).Select(x => x!.Value);
    }

    /// <summary>
    /// Check if the <see cref="Style.Modifier"/> contains the specified value.
    /// </summary>
    /// <param name="e"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool Contains(this Style.Modifier e, Style.Modifier t)
    {
        return t switch
        {
            Style.Modifier.Default => e == Style.Modifier.Default,
            Style.Modifier.None => e == Style.Modifier.None,
            _ => (e & t) == t
        };
    }

    /// <summary>
    /// Divide the <see cref="Style.Modifier"/> into single flag values.
    /// </summary>
    public static IEnumerable<Style.Modifier> DivideSingles(this Style.Modifier e)
    {
        return Enum.GetValues<Style.Modifier>().Where(x => e.Contains(x));
    }
}