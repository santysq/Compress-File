using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace PSCompression;

public class EncodingCompleter : IArgumentCompleter
{
    private static readonly string[] _encodingSet =
    {
        "ascii",
        "bigendianutf32",
        "unicode",
        "utf8",
        "utf8NoBOM",
        "bigendianunicode",
        "oem",
        "utf7",
        "utf8BOM",
        "utf32",
        "ansi"
    };

    public IEnumerable<CompletionResult> CompleteArgument(
        string commandName,
        string parameterName,
        string wordToComplete,
        CommandAst commandAst,
        IDictionary fakeBoundParameters)
    {
        foreach (string encoding in _encodingSet)
        {
            yield return new CompletionResult(encoding);
        }
    }
}
