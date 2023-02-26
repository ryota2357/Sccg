using System;
using System.Collections.Generic;
using System.Linq;

namespace Sccg.Builtin.Develop;

public static class StdFormatterImpl
{
    public static IEnumerable<string> CreateHeader(Metadata metadata, string? commentPrefix)
    {
        var header = metadata.Header(metadata);

        if (header is not null)
        {
            return AddCommentPrefix(header, commentPrefix);
        }

        var data = new[]
            {
                ("Name", metadata.ThemeName),
                ("Version", metadata.Version),
                ("Author", metadata.Author),
                ("Maintainer", metadata.Maintainer),
                ("License", metadata.License),
                ("Description", metadata.Description),
                ("Homepage", metadata.Homepage),
                ("Repository", metadata.Repository),
                ("Last change",
                    metadata.LastUpdated?.ToString("yyyy-MM-dd dddd",
                        System.Globalization.CultureInfo.CreateSpecificCulture("en-US")))
            }.Where(x => x.Item2 is not null)
             .OfType<(string, string)>()
             .ToArray();
        if (data.Length != 0)
        {
            var maxLen = data.Max(x => x.Item1.Length);
            header = data.Select(x => $"{(x.Item1 + ":").PadRight(maxLen + 1, ' ')} {x.Item2}")
                         .ToArray();
        }
        else
        {
            header = Array.Empty<string>();
        }

        return AddCommentPrefix(header, commentPrefix);
    }

    public static IEnumerable<string> CreateFooter(Metadata metadata, string? commentPrefix)
    {
        var footer = metadata.Footer(metadata) ?? new[] { $"Built with Sccg {Metadata.__SccgVersion}" };
        return AddCommentPrefix(footer, commentPrefix);
    }

    private static IEnumerable<string> AddCommentPrefix(IEnumerable<string?> lines, string? commentPrefix)
    {
        var cp = commentPrefix is null ? "" : commentPrefix + " ";
        return lines.Select(x => x is null ? cp : $"{cp}{x}");
    }
}