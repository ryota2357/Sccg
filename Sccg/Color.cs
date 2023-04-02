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
    /// Represents the default color.
    /// </summary>
    /// <remarks>This color depends on the platform (formatter).</remarks>
    public static Color Default => new(_default);

    /// <summary>
    /// Represents the no color.
    /// </summary>
    /// <remarks>This color depends on the platform (formatter).</remarks>
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
    /// Check if the color is <see cref="Default"/>.
    /// </summary>
    /// <returns>ture if it is <see cref="Default">Color.Default</see>.</returns>
    public bool IsDefault()
    {
        return HexCode == _default;
    }

    /// <summary>
    /// Check if the color is <see cref="None"/>.
    /// </summary>
    /// <returns>ture if it is <see cref="None">Color.None</see>.</returns>
    public bool IsNone()
    {
        return HexCode == _none;
    }

    /// <summary>
    /// Check if the color has hex code. (not <see cref="IsDefault"/> and not <see cref="IsNone"/>)
    /// </summary>
    /// <returns>ture if it has hex code.</returns>
    public bool HasHexCode()
    {
        return !IsDefault() && !IsNone();
    }

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
    /// Blend two colors.
    /// </summary>
    /// <param name="foreground">Added color to blend.</param>
    /// <param name="background">Base color to blend.</param>
    /// <param name="alpha">Blend rate: 0 to 1 (0 is not blend, 1 is full blend).</param>
    /// <returns>Blended color.</returns>
    /// <exception cref="ArgumentException"><paramref name="foreground"/> or <paramref name="background"/> is Color.Default or Color.None</exception>
    public static Color AlphaBlend(Color foreground, Color background, float alpha)
    {
        if (foreground.IsDefault() || background.IsDefault() || foreground.IsNone() || background.IsNone())
        {
            throw new ArgumentException("Color.Default and Color.None cannot be used for blending.");
        }

        alpha = Math.Clamp(alpha, 0, 1);
        var fg = foreground.HexCode[1..];
        var bg = background.HexCode[1..];

        var rgb = new byte[3];
        for (var i = 0; i < 3; i++)
        {
            var f = Convert.ToByte(fg[(2 * i)..(2 * (i + 1))], 16);
            var b = Convert.ToByte(bg[(2 * i)..(2 * (i + 1))], 16);
            rgb[i] = (byte)Math.Round(b + (f - b) * alpha);
        }
        return new Color(rgb[0], rgb[1], rgb[2]);
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