using System;
using System.Text.RegularExpressions;

namespace Sccg;

public readonly partial struct Style : IEquatable<Style>
{
    public Color Foreground { get; }

    public Color Background { get; }

    public Color Special { get; }

    public Modifier Modifiers { get; }

    public static Style Default => new();

    public Style()
    {
        Foreground = Color.Default;
        Background = Color.Default;
        Special = Color.Default;
        Modifiers = Modifier.Default;
    }

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
        Modifiers |= bold ? Modifier.Bold : 0;
        Modifiers |= italic ? Modifier.Italic : 0;
        Modifiers |= strikethrough ? Modifier.Strikethrough : 0;
        Modifiers |= underline ? Modifier.Underline : 0;
        Modifiers |= underlineWaved ? Modifier.UnderlineWaved : 0;
        Modifiers |= underlineDotted ? Modifier.UnderlineDotted : 0;
        Modifiers |= underlineDashed ? Modifier.UnderlineDashed : 0;
        Modifiers |= underlineDouble ? Modifier.UnderlineDouble : 0;
    }

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

    public bool Equals(Style other)
    {
        return Foreground.Equals(other.Foreground) &&
               Background.Equals(other.Background) &&
               Special.Equals(other.Special) &&
               Modifiers == other.Modifiers;
    }

    public override bool Equals(object? obj)
    {
        return obj is Style other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Foreground, Background, Special, (int)Modifiers);
    }

    public static bool operator ==(Style left, Style right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Style left, Style right)
    {
        return !(left == right);
    }

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