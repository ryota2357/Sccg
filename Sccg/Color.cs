using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Sccg;

/// <summary>
/// Represents a hex color.
/// </summary>
public readonly partial struct Color : IEquatable<Color>
{
    private const string _default = "__DEFAULT__";
    private const string _none = "__NONE__";

    /// <summary>
    /// TODO: doc
    /// </summary>
    public static Color Default => new(_default);

    /// <summary>
    /// TODO: doc
    /// </summary>
    public static Color None => new(_none);

    /// <summary>
    /// The hex code of the color which starts with '#' and followed by 6 hex digits (lower character).
    /// </summary>
    /// <remarks>
    /// This will be "DEFAULT" or "NONE" if it is <see cref="Default">Color.Default</see> or <see cref="None">Color.None</see>.
    /// You can use <see cref="IsDefault"/> and <see cref="IsNone"/> for checking.
    /// </remarks>
    public string HexCode { get; }

    /// <summary>
    /// The terminal color code. the default is null.
    /// </summary>
    /// <remarks>
    /// This is optional property. This is not used in comparison(==, Equal()) and GetHashCode().
    /// </remarks>
    public byte? TerminalColorCode { get; } = null;

    /// <summary>
    /// Check if the color is default.
    /// </summary>
    /// <returns>ture if it is <see cref="Default">Color.Default</see>.</returns>
    public bool IsDefault => HexCode == _default;

    /// <summary>
    /// TODO:
    /// </summary>
    /// <returns>ture if it is <see cref="None">Color.None</see>.</returns>
    public bool IsNone => HexCode == _none;

    /// <summary>
    /// Initialize a new instance of <see cref="Color"/> with the specified hex code.
    /// </summary>
    /// <param name="hexCode">3 or 6 hex digits (with or without # as the first character).</param>.
    /// <exception cref="ArgumentException"><paramref name="hexCode"/> is invalid hex code style.</exception>
    public Color(string hexCode)
    {
        if (hexCode is _default or _none)
        {
            HexCode = hexCode;
            return;
        }

        if (Color6Digits().IsMatch(hexCode))
        {
            HexCode = (hexCode.StartsWith("#") ? hexCode : "#" + hexCode).ToLower();
            return;
        }

        if (Color3Digits().IsMatch(hexCode))
        {
            hexCode = (hexCode.StartsWith("#") ? hexCode : "#" + hexCode).ToLower();
            HexCode = new StringBuilder()
                      .Append(hexCode[0])
                      .Append(hexCode[1])
                      .Append(hexCode[1])
                      .Append(hexCode[2])
                      .Append(hexCode[2])
                      .Append(hexCode[3])
                      .Append(hexCode[3])
                      .ToString();
            return;
        }

        throw new ArgumentException("Invalid hex code.", nameof(hexCode));
    }

    /// <summary>
    /// Initialize a new instance of <see cref="Color"/> with the specified hex code and terminal color code.
    /// </summary>
    /// <param name="hexCode">3 or 6 hex digits (with or without # as the first character).</param>.
    /// <param name="terminalColorCode">Optional terminal code.</param>
    public Color(string hexCode, byte terminalColorCode)
        : this(hexCode)
    {
        TerminalColorCode = terminalColorCode;
    }

    /// <summary>
    /// Initialize a new instance of <see cref="Color"/> with the specified RGB values.
    /// </summary>
    /// <param name="r">Red value.</param>
    /// <param name="g">Green value.</param>
    /// <param name="b">Blue value.</param>
    public Color(byte r, byte g, byte b)
    {
        HexCode = $"#{r:x2}{g:x2}{b:x2}";
    }

    /// <summary>
    /// Initialize a new instance of <see cref="Color"/> with the specified RGB values and terminal color code.
    /// </summary>
    /// <param name="r">Red value.</param>
    /// <param name="g">Green value.</param>
    /// <param name="b">Blue value.</param>
    /// <param name="terminalColorCode">Optional terminal code.</param>
    public Color(byte r, byte g, byte b, byte terminalColorCode)
        : this(r, g, b)
    {
        TerminalColorCode = terminalColorCode;
    }

    /// <inheritdoc cref="Color(string)"/>
    public static implicit operator Color(string color)
    {
        return new Color(color);
    }

    /// <inheritdoc cref="Color(byte, byte, byte)"/>
    public static implicit operator Color((byte, byte, byte) rgb)
    {
        var (r, g, b) = rgb;
        return new Color(r, g, b);
    }

    /// <summary>
    /// Converts this <see cref="Color"/> structure to a human-readable string.
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder("Color(");

        sb.Append(HexCode is _default or _none ? HexCode.ToLower()[2..^2] : HexCode);
        if (TerminalColorCode is not null)
        {
            sb.Append(',');
            sb.Append(TerminalColorCode);
        }
        sb.Append(')');

        return sb.ToString();
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    public bool Equals(Color other)
    {
        return HexCode == other.HexCode;
    }

    /// <summary>
    /// Tests whether the specified object is a <see cref="Color"/> structure and is equivalent to this <see cref="Color"/> structure.
    /// </summary>
    /// <param name="obj">The object to test.</param>
    public override bool Equals(object? obj)
    {
        return obj is Color other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        if (HexCode == Default) return -1;
        if (HexCode == None) return -2;
        var r = int.Parse(HexCode[1..3], NumberStyles.HexNumber);
        var g = int.Parse(HexCode[3..5], NumberStyles.HexNumber);
        var b = int.Parse(HexCode[5..7], NumberStyles.HexNumber);
        return (r << 16) + (g << 8) + b;
    }

    /// <summary>
    /// Tests whether two specified <see cref="Color"/> structures are equivalent.
    /// </summary>
    public static bool operator ==(in Color left, in Color right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Tests whether two specified <see cref="Color"/> structures are different.
    /// </summary>
    public static bool operator !=(in Color left, in Color right)
    {
        return !(left == right);
    }

    [GeneratedRegex("^#?[0-9A-Fa-f]{6}$")]
    private static partial Regex Color6Digits();

    [GeneratedRegex("^#?[0-9A-Fa-f]{3}$")]
    private static partial Regex Color3Digits();
}