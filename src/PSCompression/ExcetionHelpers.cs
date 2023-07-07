using System;
using System.Management.Automation;

namespace PSCompression;

internal static class ExceptionHelpers
{
    internal static ErrorRecord NotArchivePathError(string path) =>
        new(new ArgumentException($"Unable to get zip content because it is a directory: '{path}'."),
            "NotArchivePath", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord NotDirectoryPathError(string path) =>
        new(new ArgumentException($"Destination path is an existing file: '{path}'."),
            "NotDirectoryPath", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord InvalidProviderError(string path, ProviderInfo provider) =>
        new(new ArgumentException($"The resolved path '{path}' is not a FileSystem path but '{provider.Name}'."),
            "NotFileSystemPath", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord ZipOpenError(string path, Exception e) =>
        new(e, "ZipOpen", ErrorCategory.OpenError, path);

    internal static ErrorRecord ResolvePathError(string path, Exception e) =>
        new(e, "ResolvePath", ErrorCategory.NotSpecified, path);

    internal static ErrorRecord ExtractEntryError(ZipEntryBase entry, Exception e) =>
        new(e, "ExtractEntry", ErrorCategory.NotSpecified, entry);

    internal static ErrorRecord StreamOpenError(ZipEntryFile entry, Exception e) =>
        new(e, "StreamOpen", ErrorCategory.NotSpecified, entry);

    internal static ErrorRecord WriteError(object entry, Exception e) =>
        new(e, "WriteError", ErrorCategory.WriteError, entry);

    internal static ErrorRecord DuplicatedEntryError(string entry, string source) =>
        new(new Exception($"An entry with path '{entry}' already exists in '{source}'."),
            "DuplicatedEntry", ErrorCategory.WriteError, entry);
}
