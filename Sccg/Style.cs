using System;
using System.Text.RegularExpressions;

namespace Sccg;

/// <summary>
/// Represents a style of text.
/// </summary>
public readonly partial struct Style : IEquatable<Style>
{
    /// <summary>
    /// Foreground color.
    /// </summary>
    public Color Foreground { get; init; }

    /// <summary>
    /// Background color.
    /// </summary>
    public Color Background { get; init; }

    /// <summary>
    /// Special color.
    /// </summary>
    public Color Special { get; init; }

    /// <summary>
    /// Modifiers.
    /// </summary>
    public Modifier Modifiers { get; private init; }

    /// <inheritdoc cref="Style.Modifier.Bold"/>
    public bool Bold
    {
        init => Modifiers = value ? (Modifiers | Modifier.Bold) : (Modifiers & ~Modifier.Bold);
    }

    /// <inheritdoc cref="Style.Modifier.Italic"/>
    public bool Italic
    {
        init => Modifiers = value ? (Modifiers | Modifier.Italic) : (Modifiers & ~Modifier.Italic);
    }

    /// <inheritdoc cref="Style.Modifier.Strikethrough"/>
    public bool Strikethrough
    {
        init => Modifiers = value ? (Modifiers | Modifier.Strikethrough) : (Modifiers & ~Modifier.Strikethrough);
    }

    /// <inheritdoc cref="Style.Modifier.Underline"/>
    public bool Underline
    {
        init => Modifiers = value ? (Modifiers | Modifier.Underline) : (Modifiers & ~Modifier.Underline);
    }

    /// <inheritdoc cref="Style.Modifier.UnderlineWaved"/>
    public bool UnderlineWaved
    {
        init => Modifiers = value ? (Modifiers | Modifier.UnderlineWaved) : (Modifiers & ~Modifier.UnderlineWaved);
    }

    /// <inheritdoc cref="Style.Modifier.UnderlineDotted"/>
    public bool UnderlineDotted
    {
        init => Modifiers = value ? (Modifiers | Modifier.UnderlineDotted) : (Modifiers & ~Modifier.UnderlineDotted);
    }

    /// <inheritdoc cref="Style.Modifier.UnderlineDashed"/>
    public bool UnderlineDashed
    {
        init => Modifiers = value ? (Modifiers | Modifier.UnderlineDashed) : (Modifiers & ~Modifier.UnderlineDashed);
    }

    /// <inheritdoc cref="Style.Modifier.UnderlineDouble"/>
    public bool UnderlineDouble
    {
        init => Modifiers = value ? (Modifiers | Modifier.UnderlineDouble) : (Modifiers & ~Modifier.UnderlineDouble);
    }

    /// <summary>
    /// Gets a value indicating whether this style is the default style.
    /// </summary>
    public static Style Default => new();

    /// <summary>
    /// Initializes a new instance of the default <see cref="Style"/> struct.
    /// </summary>
    public Style()
    {
        Foreground = Color.Default;
        Background = Color.Default;
        Special = Color.Default;
        Modifiers = Modifier.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Style"/> struct.
    /// </summary>
    public Style(
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
        Foreground = fg ?? Color.Default;
        Background = bg ?? Color.Default;
        Special = sp ?? Color.Default;

        if (none)
        {
            Modifiers = Modifier.None;
            return;
        }

        Modifiers = Modifier.Default;
        Bold = bold;
        Italic = italic;
        Strikethrough = strikethrough;
        Underline = underline;
        UnderlineWaved = underlineWaved;
        UnderlineDotted = underlineDotted;
        UnderlineDashed = underlineDashed;
        UnderlineDouble = underlineDouble;
    }

    /// <summary>
    /// Converts this <see cref="Style"/> structure to a human-readable string.
    /// </summary>
    public override string ToString()
    {
        // 0123456...^1
        // Color(#...)
        var fg = Foreground.ToString()[6..^1];
        var bg = Background.ToString()[6..^1];
        var sp = Special.ToString()[6..^1];
        var mod = UpperCharRegex().Replace(Modifiers.ToString(), "_$1").ToLower()[1..];
        return $"Style(fg:{fg},bg:{bg},sp:{sp},{mod})";
    }

    /// <inheritdoc />
    public bool Equals(Style other)
    {
        return Foreground.Equals(other.Foreground) &&
               Background.Equals(other.Background) &&
               Special.Equals(other.Special) &&
               Modifiers == other.Modifiers;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Style other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Foreground, Background, Special, (int)Modifiers);
    }

    /// <summary>
    /// Tests whether two specified <see cref="Style"/> structures are equivalent.
    /// </summary>
    public static bool operator ==(Style left, Style right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Tests whether two specified <see cref="Style"/> structures are different.
    /// </summary>
    public static bool operator !=(Style left, Style right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Sets of decorations.
    /// </summary>
    [Flags]
    public enum Modifier
    {
        /// <summary>
        /// Not specify any decoration.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Reset decoration.
        /// </summary>
        None = 1 << 0,

        /// <summary>
        /// Bold text.
        /// </summary>
        Bold = 1 << 1,

        /// <summary>
        /// Italic text.
        /// </summary>
        Italic = 1 << 2,

        /// <summary>
        /// Strikethrough text.
        /// </summary>
        Strikethrough = 1 << 3,

        /// <summary>
        /// Underline text.
        /// </summary>
        Underline = 1 << 4,

        /// <summary>
        /// Under waved text.
        /// </summary>
        UnderlineWaved = 1 << 5,

        /// <summary>
        /// Under dotted text.
        /// </summary>
        UnderlineDotted = 1 << 6,

        /// <summary>
        /// Under dashed text.
        /// </summary>
        UnderlineDashed = 1 << 7,

        /// <summary>
        /// Under double line text.
        /// </summary>
        UnderlineDouble = 1 << 8
    }

    [GeneratedRegex("([A-Z])")]
    private static partial Regex UpperCharRegex();
}