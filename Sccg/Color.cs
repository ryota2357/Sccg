using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Sccg;

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

    public Color(string hexCode, byte terminalColorCode)
        : this(hexCode)
    {
        TerminalColorCode = terminalColorCode;
    }

    public Color(byte r, byte g, byte b)
    {
        HexCode = $"#{r:x2}{g:x2}{b:x2}";
    }

    public Color(byte r, byte g, byte b, byte terminalColorCode)
        : this(r, g, b)
    {
        TerminalColorCode = terminalColorCode;
    }

    public static implicit operator Color(string color)
    {
        return new Color(color);
    }

    public static implicit operator Color((byte, byte, byte) rgb)
    {
        var (r, g, b) = rgb;
        return new Color(r, g, b);
    }

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

    public bool Equals(Color other)
    {
        return HexCode == other.HexCode;
    }

    public override bool Equals(object? obj)
    {
        return obj is Color other && Equals(other);
    }

    public override int GetHashCode()
    {
        if (HexCode == Default) return -1;
        if (HexCode == None) return -2;
        var r = int.Parse(HexCode[1..3], NumberStyles.HexNumber);
        var g = int.Parse(HexCode[3..5], NumberStyles.HexNumber);
        var b = int.Parse(HexCode[5..7], NumberStyles.HexNumber);
        return (r << 16) + (g << 8) + b;
    }

    public static bool operator ==(in Color left, in Color right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(in Color left, in Color right)
    {
        return !(left == right);
    }

    [GeneratedRegex("^#?[0-9A-Fa-f]{6}$")]
    private static partial Regex Color6Digits();

    [GeneratedRegex("^#?[0-9A-Fa-f]{3}$")]
    private static partial Regex Color3Digits();
}