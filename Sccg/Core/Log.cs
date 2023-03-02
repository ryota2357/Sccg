using System;
using System.Linq;
using System.Text;
using Kokuban;
using Kokuban.AnsiEscape;

namespace Sccg.Core;

/// <summary>
/// The logger class.
/// </summary>
public static class Log
{
    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="msg">A message. If multiple messages are passed, they will be added line-break.</param>
    public static void Debug(params string[] msg)
    {
        if (LoggerConfig.Shared.Level <= LogLevel.Debug)
        {
            PrintLine(Chalk.Blue["Debug"], 5, msg);
        }
    }

    /// <summary>
    /// Logs an information message.
    /// </summary>
    /// <param name="msg">A message. If multiple messages are passed, they will be added line-break.</param>
    public static void Info(params string[] msg)
    {
        if (LoggerConfig.Shared.Level <= LogLevel.Info)
        {
            PrintLine(Chalk.Green["Info"], 4, msg);
        }
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="msg">A message. If multiple messages are passed, they will be added line-break.</param>
    public static void Warn(params string[] msg)
    {
        if (LoggerConfig.Shared.Level <= LogLevel.Warn)
        {
            PrintLine(Chalk.Yellow["Warn"], 4, msg);
        }
    }

    private static void PrintLine(AnsiStyledString prefix, int prefixLength, params string[] msg)
    {
        var indent = new string(' ', prefixLength + 1);
        var text = msg.Length switch
        {
            1 => prefix + " " + msg[0],
            > 1 => new StringBuilder(prefix + " " + msg[0])
                   .AppendJoin(Environment.NewLine + indent, msg.Skip(1))
                   .ToString(),
            _ => ""
        };
        Console.WriteLine(text);
        // TODO: implement for log file
    }
}

internal class LoggerConfig
{
    internal static readonly LoggerConfig Shared = new();

    public LogLevel Level { get; set; } = LogLevel.Info;

    public string? File { get; set; } = null;
}