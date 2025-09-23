using System.CommandLine;
using NuGet.Common;
using NuGetMerge;

Argument<string[]> inputOption = new("INPUT_FILES")
{
    Description = "One or more .nupkg files to merge. Files are merged in the order they are provided.",
    Arity = ArgumentArity.OneOrMore,
};
inputOption.AcceptLegalFilePathsOnly().AcceptExistingFilesOnly();

Option<string?> outputOption = new("--output", "-o")
{
    HelpName = "OUTPUT_FILE",
    Description = "Path to the output .nupkg file. If not specified, the first input file is overwritten.",
    Arity = ArgumentArity.ExactlyOne,
};
outputOption.AcceptLegalFilePathsOnly();

Option<bool> forceOption = new("--force", "-f")
{
    Description = "Overwrite the output file if it already exists.",
    Arity = ArgumentArity.Zero,
};

Option<bool> deterministicOption = new("--deterministic", "-d")
{
    Description = "Produce a deterministic package merge.",
    Arity = ArgumentArity.Zero,
};

Option<bool> quietOption = new("--quiet", "-q")
{
    Description = "Suppress all output.",
    Arity = ArgumentArity.Zero,
};

Option<LogLevel?> verbosityOption = new("--verbosity", "--verbose", "-v")
{
    HelpName = "LEVEL",
    Description = "Set the verbosity level. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].",
    Arity = ArgumentArity.ZeroOrOne,
};
verbosityOption.UseCustomParser();

RootCommand root = new("Merges multiple .nupkg files into a single package.")
{
    inputOption,
    outputOption,
    forceOption,
    deterministicOption,
    quietOption,
    verbosityOption,
};
root.FixPunctuation();

root.SetAction(args =>
{
    const string stdFileName = "-";
    string[] input = args.GetRequiredValue(inputOption);
    string output = args.GetValue(outputOption) ?? (Console.IsOutputRedirected ? stdFileName : input[0]);
    bool force = args.GetValue(forceOption);
    bool deterministic = args.GetValue(deterministicOption);
    bool quiet = args.GetValue(quietOption);
    LogLevel verbosity = quiet ? (LogLevel.Error + 1) : (args.GetValue(verbosityOption) ?? LogLevel.Information);

    // Verbosity levels higher than `Error` mean "don't print anything".
    // However, we disregard this sentiment later if the user explicitly asks to print
    // the nupkg to stdout, since otherwise running the tool would make absolutely no sense.
    bool ignoreOutput = verbosity > LogLevel.Error;
    if (ignoreOutput)
        args.InvocationConfiguration.Output = args.InvocationConfiguration.Error = TextWriter.Null;

    // Never override a file without the user's explicit consent, and never override a directory period.
    if (output != stdFileName && (!force && File.Exists(output) || Directory.Exists(output)))
        return args.Error($"File or directory '{output}' already exists.");

    IEnumerable<Stream> packages = input.Select(x => x == stdFileName ? Console.OpenStandardInput() : File.OpenRead(x));
    TextLogger logger = new(args.InvocationConfiguration.Error, verbosity);
    NuGetPackageBuilder mergedPackage = NuGetPackageBuilder.Merge(packages, deterministic, logger);

    using Stream outputStream = output == stdFileName ? Console.OpenStandardOutput() : File.Create(output);
    mergedPackage.Save(outputStream);
    return 0;
});
return root.Parse(args).Invoke();
