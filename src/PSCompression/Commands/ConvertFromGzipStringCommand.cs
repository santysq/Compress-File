using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;
using PSCompression.Exceptions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertFrom, "GzipString")]
[OutputType(typeof(string))]
[Alias("gzipfromstring")]
public sealed class ConvertFromGzipStringCommand : PSCmdlet
{
    [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
    public string[] InputObject { get; set; } = null!;

    [Parameter]
    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    [ValidateNotNullOrEmpty]
    public Encoding Encoding { get; set; } = new UTF8Encoding();

    [Parameter]
    public SwitchParameter Raw { get; set; }

    protected override void ProcessRecord()
    {
        foreach (string line in InputObject)
        {
            try
            {
                using MemoryStream inStream = new(Convert.FromBase64String(line));
                using GZipStream gzip = new(inStream, CompressionMode.Decompress);
                using StreamReader reader = new(gzip, Encoding);

                if (Raw.IsPresent)
                {
                    WriteObject(reader.ReadToEnd());
                    return;
                }

                while (!reader.EndOfStream)
                {
                    WriteObject(reader.ReadLine());
                }
            }
            catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception exception)
            {
                WriteError(exception.ToEnumerationError(line));
            }
        }
    }
}
