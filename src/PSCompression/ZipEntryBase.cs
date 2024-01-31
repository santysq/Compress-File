using System;
using System.IO;
using System.IO.Compression;

namespace PSCompression;

public enum ZipEntryType
{
    Directory = 0,
    Archive = 1
}

public abstract class ZipEntryBase
{
    public string Source { get; }

    public string Name { get; }

    public string RelativePath { get; }

    public DateTime LastWriteTime { get; }

    public long? Length { get; internal set; }

    public long? CompressedLength { get; internal set; }

    public abstract ZipEntryType Type { get; }

    protected ZipEntryBase(ZipArchiveEntry entry, string source)
    {
        Source = source;
        Name = entry.Name;
        RelativePath = entry.FullName;
        LastWriteTime = entry.LastWriteTime.LocalDateTime;
        Length = entry.Length;
        CompressedLength = entry.CompressedLength;
    }

    public void Remove()
    {
        using ZipArchive zip = ZipFile.Open(Source, ZipArchiveMode.Update);
        zip.GetEntry(RelativePath)?.Delete();
    }

    internal void Remove(ZipArchive zip) =>
        zip.GetEntry(RelativePath)?
        .Delete();

    internal static string Move(
        string path,
        string destination,
        string source,
        ZipArchive zip)
    {
        zip.ThrowIfNotFound(
            path: path,
            source: source,
            entry: out ZipArchiveEntry sourceEntry);

        zip.ThrowIfDuplicate(
            path: destination,
            source: source);

        destination.ThrowIfInvalidPathChar();

        ZipArchiveEntry destinationEntry = zip.CreateEntry(destination);
        using (Stream sourceStream = sourceEntry.Open())
        using (Stream destinationStream = destinationEntry.Open())
        {
            sourceStream.CopyTo(destinationStream);
        }
        sourceEntry.Delete();

        return destination;
    }

    internal ZipArchive Open(ZipArchiveMode mode) =>
        ZipFile.Open(Source, mode);

    public FileSystemInfo ExtractTo(string destination, bool overwrite)
    {
        using ZipArchive zip = ZipFile.OpenRead(Source);
        (string path, bool isArchive) = this.ExtractTo(zip, destination, overwrite);

        if (isArchive)
        {
            return new FileInfo(path);
        }

        return new DirectoryInfo(path);
    }

    public override string ToString() => RelativePath;
}
