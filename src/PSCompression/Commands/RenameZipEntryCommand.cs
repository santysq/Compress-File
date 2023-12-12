using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Rename, "ZipEntry", SupportsShouldProcess = true)]
public sealed class RenameZipEntryCommand : PSCmdlet, IDisposable
{
    private readonly ZipArchiveCache _cache = new(ZipArchiveMode.Update);

    private Dictionary<string, List<string>>? _output;

    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true)]
    public ZipEntryBase ZipEntry { get; set; } = null!;

    [Parameter(
        Mandatory = true,
        Position = 1,
        ValueFromPipeline = true)]
    public string NewName { get; set; } = null!;

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        if (PassThru.IsPresent)
        {
            _output = new();
        }
    }

    protected override void ProcessRecord()
    {
        if (!ShouldProcess(target: ZipEntry.ToString(), action: "Rename"))
        {
            return;
        }

        try
        {
            string destination = ZipEntry.Rename(
                newname: NewName,
                zip: _cache.GetOrAdd(ZipEntry));

            if (!PassThru.IsPresent || _output is null)
            {
                return;
            }

            if (!_output.ContainsKey(ZipEntry.Source))
            {
                _output[ZipEntry.Source] = new();
            }

            _output[ZipEntry.Source].Add(destination);
        }
        catch (Exception e) when (e is PipelineStoppedException or FlowControlException)
        {
            throw;
        }
        catch (DuplicatedEntryException e)
        {
            WriteError(ExceptionHelpers.DuplicatedEntryError(e));
        }
        catch (EntryNotFoundException e)
        {
            WriteError(ExceptionHelpers.EntryNotFoundError(e));
        }
        catch (ArgumentException e)
        {
            WriteError(ExceptionHelpers.InvalidNameError(NewName, e));
        }
        catch (Exception e)
        {
            WriteError(ExceptionHelpers.WriteError(ZipEntry, e));
        }
    }

    protected override void EndProcessing()
    {
        if (!PassThru.IsPresent || _output is null)
        {
            return;
        }

        _cache?.Dispose();

        foreach (ZipEntryBase entry in _output)
        {
            if (entry is ZipEntryFile entryFile)
            {
                entryFile.Refresh(_cacheOut.GetOrAdd(entry));
            }

            WriteObject(entry);
        }
    }

    public void Dispose()
    {
        _cache?.Dispose();
        _cacheOut?.Dispose();
    }
}
