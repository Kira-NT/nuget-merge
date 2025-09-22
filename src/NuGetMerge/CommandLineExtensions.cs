using System.CommandLine;
using System.CommandLine.Parsing;
using NuGet.Common;

namespace NuGetMerge;

internal static class CommandLineExtensions
{
    public static int Error(this ParseResult parseResult, string? message)
    {
        TextWriter stderr = parseResult.InvocationConfiguration.Error;
        if (Console.IsErrorRedirected)
        {
            stderr.WriteLine(message);
        }
        else
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            stderr.WriteLine(message);
            Console.ResetColor();
        }
        return 1;
    }

    public static void FixPunctuation(this Symbol symbol)
    {
        // Some symbols provided by System.CommandLine by default (e.g., HelpOption)
        // don't have a period at the end of their description.
        // For consistency's sake, let's fix that up.
        if (symbol.Description is { Length: > 0 } desc && desc[desc.Length - 1] != '.')
            symbol.Description = desc + ".";
    }

    public static void FixPunctuation(this Command command)
    {
        ((Symbol)command).FixPunctuation();

        foreach (Option options in command.Options)
            options.FixPunctuation();

        foreach (Argument argument in command.Arguments)
            argument.FixPunctuation();
    }

    public static Argument<T> AcceptExistingFilesOnly<T>(this Argument<T> argument)
    {
        argument.Validators.Add(EnsureFilesExist);
        return argument;
    }

    private static void EnsureFilesExist(ArgumentResult argument)
    {
        for (int i = 0; i < argument.Tokens.Count; i++)
        {
            // According to POSIX, "-" represents stdin/stdout (depending on the context),
            // so always treat it as an existing file.
            string token = argument.Tokens[i].Value;
            if (token != "-" && !File.Exists(token))
                argument.AddError($"File does not exist: '{token}'.");
        }
    }

    public static Option<LogLevel?> UseCustomParser(this Option<LogLevel?> option)
    {
        option.CustomParser = ParseLogLevel;
        return option;
    }

    private static LogLevel? ParseLogLevel(ArgumentResult argument)
    {
        if (argument.Tokens.Count == 0)
            return LogLevel.Verbose;

        string token = argument.Tokens[0].Value;
        switch (token)
        {
            case "q" or "quiet":
                // If we don't even output errors, then we don't output anything.
                return LogLevel.Error + 1;

            case "d" or "detailed":
                return LogLevel.Verbose;

            case "m" or "minimal":
                return LogLevel.Warning;

            case "n" or "normal":
                return LogLevel.Information;

            case "diag" or "diagnostic":
                return LogLevel.Debug;

            default:
                argument.AddError($"Invalid verbosity level '{token}'. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].");
                return null;
        }
    }
}
