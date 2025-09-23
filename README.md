# nuget-merge

[![Version](https://img.shields.io/github/v/release/Kira-NT/nuget-merge?sort=date&label=version)](https://github.com/Kira-NT/nuget-merge/releases/latest)
[![License](https://img.shields.io/github/license/Kira-NT/nuget-merge?cacheSeconds=36000)](https://github.com/Kira-NT/nuget-merge/blob/HEAD/LICENSE.md)

<img alt="Icon" src="https://raw.githubusercontent.com/Kira-NT/nuget-merge/HEAD/media/icon.png" width="128">

`nuget-merge` is a small, POSIX-friendly .NET tool that lets you merge multiple `.nupkg` files into a single NuGet package.

For example, if you build .NET Standard 2.0 and .NET Standard 2.1 versions of your library from different source trees, or if you build a tool or an analyzer separately from the library it should be shipped with, `nuget-merge` can combine the resulting packages into one, so you don't need to distribute them separately. Admittedly, this is a fairly niche scenario, and you'll probably never find yourself in one, but when you *do* need something like this, you *really* need it.

This tool, however, is **not** intended for bundling dependencies with your package. If that's what you are looking for, please see [the official MSBuild docs on the topic](https://learn.microsoft.com/en-us/visualstudio/msbuild/tutorial-custom-task-code-generation?view=vs-2022#bundle-dependencies-into-the-package). There's no need to use external tools for that.

----

## Installation

`nuget-merge` is available as a [.NET tool](https://www.nuget.org/packages/nuget-merge), which you can install globally:

```bash
dotnet tool install --global nuget-merge
# nuget-merge in.1.nupkg in.2.nupkg -o out.nupkg
```

Or locally:

```bash
dotnet new tool-manifest # If you haven't created a tool manifest yet.
dotnet tool install --local nuget-merge
# dotnet tool run nuget-merge in.1.nupkg in.2.nupkg -o out.nupkg
```

Alternatively, `nuget-merge` is also available as a framework-independent precompiled binary for a small selection of popular platforms, which you can download from [the latest release](https://github.com/Kira-NT/nuget-merge/releases/latest).

----

## Usage

```
Description:
  Merges multiple .nupkg files into a single package.

Usage:
  nuget-merge <INPUT_FILES>... [options]

Examples:
  nuget-merge in.1.nupkg in.2.nupkg in.3.nupkg -o out.nupkg
  nuget-merge dist/*.nupkg > out.nupkg
  nuget-merge -fd in.out.nupkg in.2.nupkg
  cat in.2.nupkg | nuget-merge in.1.nupkg - in.3.nupkg > out.nupkg

Arguments:
  <INPUT_FILES>  One or more .nupkg files to merge. Files are merged in the order they are provided.

Options:
  -?, -h, --help                      Show help and usage information.
  --version                           Show version information.
  -o, --output <OUTPUT_FILE>          Path to the output .nupkg file. If not specified, the first input file is overwritten.
  -f, --force                         Overwrite the output file if it already exists.
  -d, --deterministic                 Produce a deterministic package merge.
  -q, --quiet                         Suppress all output.
  -v, --verbose, --verbosity <LEVEL>  Set the verbosity level. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].
```

----

## License

Licensed under the terms of the [MIT License](https://github.com/Kira-NT/nuget-merge/blob/HEAD/LICENSE.md).
