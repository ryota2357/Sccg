using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sccg.Builtin.Sources.Internal;
using Sccg.Builtin.Writers;
using Sccg.Core;
using Sccg.Utility;

namespace Sccg.Builtin.Formatters;

/// <summary>
/// SourceItem for VSCode color theme json (colors).
/// </summary>
public interface IVSCodeColorSourceItem : IVSCodeSourceItemBase
{
    /// <summary>
    /// Extracts <see cref="VSCodeFormatter.ColorFormattable"/> from this item.
    /// </summary>
    public VSCodeFormatter.ColorFormattable Extract();
}

/// <summary>
/// SourceItem for VSCode color theme json (tokenColors).
/// </summary>
public interface IVSCodeTokenColorSourceItem : IVSCodeSourceItemBase
{
    /// <summary>
    /// Extracts <see cref="VSCodeFormatter.TokenColorFormattable"/> from this item.
    /// </summary>
    public VSCodeFormatter.TokenColorFormattable Extract();
}

/// <summary>
/// VSCode color theme with Json.
/// </summary>
public class VSCodeFormatter : Formatter<IVSCodeSourceItemBase, MultiTextContent>
{
    /// <inheritdoc />
    public override string Name => "VSCode";

    private const string _contextKey_vscode_displayName = "vscode.displayName";
    private const string _contextKey_vscode_engines = "vscode.engines";

    /// <inheritdoc />
    protected override MultiTextContent Format(IEnumerable<IVSCodeSourceItemBase> items, BuilderQuery query)
    {
        var content = new MultiTextContent("");
        var metadata = query.GetMetadata();

        var packageJson = CreatePackageJson(metadata);
        content.Add(packageJson);

        var themeJson = CreateThemeJson(items.ToArray(), metadata);
        content.Add(themeJson);

        return content;
    }

    private static SingleTextContent CreateThemeJson(IVSCodeSourceItemBase[] items, Metadata metadata)
    {
        var sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine($"  \"name\": \"{metadata.Context.Get(_contextKey_vscode_displayName, metadata.ThemeName) ?? "unknown"}\",");
        sb.AppendLine($"  \"type\": \"{(metadata.ThemeType == ThemeType.Light ? "light" : "dark")}\",");

        // colors
        sb.AppendLine("  \"colors\": {");
        foreach (var item in items.OfType<IVSCodeColorSourceItem>())
        {
            var data = item.Extract();
            sb.Append("    ");
            sb.AppendLine($"\"{data.Name}\": \"{data.Color.HexCode}\",");
        }
        sb.Remove(sb.Length - 2, 1); // remove last comma
        sb.AppendLine("  },");

        // tokenColors
        sb.AppendLine("  \"tokenColors\": [");
        var tokenColors = items.OfType<IVSCodeTokenColorSourceItem>()
                               .Select(i => i.Extract())
                               .GroupBy(i => i.Style)
                               .ToArray();
        foreach (var item in tokenColors)
        {
            sb.AppendLine("    {");
            if (item.Count() == 1)
            {
                var scope = item.First().Name;
                sb.AppendLine($"      \"scope\": \"{scope}\",");
            }
            else
            {
                sb.AppendLine("      \"scope\": [");
                foreach (var scope in item)
                {
                    sb.AppendLine($"        \"{scope.Name}\",");
                }
                sb.Remove(sb.Length - 2, 1); // remove last comma
                sb.AppendLine("      ],");
            }
            sb.AppendLine("      \"settings\": {");
            var style = item.Key;
            var hasSettings = false;
            if (style.Foreground.HasHexCode())
            {
                sb.AppendLine($"        \"foreground\": \"{style.Foreground.HexCode}\",");
                hasSettings = true;
            }
            // NOTE: VSCode does not support `background` for `tokenColors`
            if (!style.Modifiers.Contains(Style.Modifier.Default) && !style.Modifiers.Contains(Style.Modifier.None))
            {
                // NOTE: only support `bold`, `strikethrough`, `underline`, `italic`
                var styles = new List<string>();
                if (style.Modifiers.Contains(Style.Modifier.Bold)) styles.Add("bold");
                if (style.Modifiers.Contains(Style.Modifier.Strikethrough)) styles.Add("strikethrough");
                if (style.Modifiers.Contains(Style.Modifier.Underline)) styles.Add("underline");
                if (style.Modifiers.Contains(Style.Modifier.Italic)) styles.Add("italic");
                if (styles.Count != 0)
                {
                    var fontStyle = string.Join(" ", styles);
                    sb.AppendLine($"        \"fontStyle\": \"{fontStyle}\",");
                    hasSettings = true;
                }
            }
            if (hasSettings)
            {
                sb.Remove(sb.Length - 2, 1); // remove last comma
            }
            sb.AppendLine("      }");
            sb.AppendLine("    },");
        }
        if (tokenColors.Any())
        {
            sb.Remove(sb.Length - 2, 1); // remove last comma
        }
        sb.AppendLine("  ]");

        sb.Append('}');
        return new SingleTextContent($"themes/{metadata.ThemeName}-color-theme.json", sb.ToString());
    }

    private static SingleTextContent CreatePackageJson(Metadata metadata)
    {
        var name = metadata.ThemeName ?? "unknown";
        var displayName = metadata.Context.Get(_contextKey_vscode_displayName, metadata.ThemeName);
        var description = metadata.Description;
        var version = metadata.Version;
        var publisher = metadata.Author;
        var license = metadata.License;
        var homepage = metadata.Homepage;
        var repository = metadata.Repository;
        var engines = metadata.Context.Get(_contextKey_vscode_engines, "*");

        var sb = new StringBuilder();

        void AppendIfNotNull(int indent, string key, string? value)
        {
            if (value is null) return;
            var indentStr = new string(' ', indent);
            sb.AppendLine($"{indentStr}\"{key}\": \"{value}\",");
        }

        sb.AppendLine("{");
        AppendIfNotNull(2, "name", name);
        AppendIfNotNull(2, "displayName", displayName);
        AppendIfNotNull(2, "description", description);
        AppendIfNotNull(2, "version", version);
        AppendIfNotNull(2, "publisher", publisher);
        AppendIfNotNull(2, "license", license);
        AppendIfNotNull(2, "homepage", homepage);
        if (repository is not null)
        {
            sb.AppendLine("  \"repository\": {");
            AppendIfNotNull(4, "type", "git");
            AppendIfNotNull(4, "url", repository);
            sb.AppendLine("  },");
        }
        sb.AppendLine("  \"engines\": {");
        AppendIfNotNull(4, "vscode", engines);
        sb.Remove(sb.Length - 2, 1); // remove last comma
        sb.AppendLine("  }");
        sb.AppendLine("}");

        return new SingleTextContent("package.json", sb.ToString()[..^1]); // remove last newline
    }

    /// <summary>
    /// Formattable for VS Code "colors".
    /// </summary>
    public readonly record struct ColorFormattable(string Name, Color Color);

    /// <summary>
    /// Formattable for VS Code "tokenColors".
    /// </summary>
    public readonly record struct TokenColorFormattable(string Name, Style Style);
}