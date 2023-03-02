namespace Sccg;

/// <summary>
/// The build log level.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Show all logs.
    /// </summary>
    Debug = 0,

    /// <summary>
    /// Show only info, warn, and error logs.
    /// </summary>
    Info = 1,

    /// <summary>
    /// Show only warn and error logs.
    /// </summary>
    Warn = 2,

    /// <summary>
    /// Show only error logs.
    /// </summary>
    Error = 3
}