using System;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

[Cmdlet(VerbsCommon.Set, "ZipEntryContent", DefaultParameterSetName = "StringValue")]
[OutputType(typeof(ZipEntryFile))]
public sealed class SetZipEntryContentCommand : PSCmdlet, IDisposable
{
    private ZipContentWriter? _zipWriter;

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public object[] Value { get; set; } = null!;

    [Parameter(Mandatory = true, Position = 0)]
    public ZipEntryFile SourceEntry { get; set; } = null!;

    [Parameter(ParameterSetName = "StringValue")]
    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    public Encoding Encoding { get; set; } = new UTF8Encoding();

    [Parameter(ParameterSetName = "ByteStream")]
    public SwitchParameter AsByteStream { get; set; }

    [Parameter(ParameterSetName = "StringValue")]
    [Parameter(ParameterSetName = "ByteStream")]
    public SwitchParameter Append { get; set; }

    [Parameter(ParameterSetName = "ByteStream")]
    public int BufferSize { get; set; } = 128000;

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        try
        {
            if (ParameterSetName == "ByteStream")
            {
                _zipWriter = new ZipContentWriter(SourceEntry, Append.IsPresent, BufferSize);
                return;
            }

            _zipWriter = new ZipContentWriter(SourceEntry, Append.IsPresent, Encoding);
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(ExceptionHelpers.StreamOpenError(SourceEntry, e));
        }
    }

    protected override void ProcessRecord()
    {
        try
        {
            if (_zipWriter is null)
            {
                return;
            }

            if (ParameterSetName == "StringValue")
            {
                _zipWriter.WriteLines(LanguagePrimitives.ConvertTo<string[]>(Value));
                return;
            }

            _zipWriter.WriteBytes(LanguagePrimitives.ConvertTo<byte[]>(Value));
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(ExceptionHelpers.WriteError(SourceEntry, e));
        }
    }

    protected override void EndProcessing()
    {
        if (!PassThru.IsPresent || _zipWriter is null)
        {
            return;
        }

        try
        {
            _zipWriter.Dispose();
            SourceEntry.Refresh();
            WriteObject(SourceEntry);
        }
        catch (Exception e)
        {
            ThrowTerminatingError(ExceptionHelpers.StreamOpenError(SourceEntry, e));
        }
    }

    public void Dispose() => _zipWriter?.Dispose();
}
