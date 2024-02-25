using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using PSCompression.Extensions;

namespace PSCompression;

public sealed class ZipEntryDirectory : ZipEntryBase
{
    private const StringComparison _comparer = StringComparison.InvariantCultureIgnoreCase;

    public override ZipEntryType Type => ZipEntryType.Directory;

    internal ZipEntryDirectory(ZipArchiveEntry entry, string source)
        : base(entry, source)
    {
        Name = entry.GetDirectoryName();
    }

    internal IEnumerable<ZipArchiveEntry> GetChilds(ZipArchive zip) =>
        zip.Entries.Where(e =>
            !string.Equals(e.FullName, RelativePath, _comparer)
            && e.FullName.StartsWith(RelativePath, _comparer));
}
