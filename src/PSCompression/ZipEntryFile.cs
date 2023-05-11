using System.IO.Compression;

namespace PSCompression;

public sealed class ZipEntryFile : ZipEntryBase
{
    public ZipEntryType EntryType => ZipEntryType.File;

    internal ZipEntryFile(ZipArchiveEntry entry, string source) :
        base(entry, source)
    { }

    public ZipEntryStream OpenRead() => new(this, ZipArchiveMode.Read);

    public ZipEntryStream OpenWrite() => new(this, ZipArchiveMode.Update);
}
