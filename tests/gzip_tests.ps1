﻿## GZIP EXAMPLES

# Example 1: Strings to Gzip compressed Base 64 encoded string

$strings = 'hello', 'world', '!'
# With positional binding
ConvertTo-GzipString $strings
# Or pipeline input, both work
$strings | ConvertTo-GzipString

# Example 2: Expanding compressed strings

ConvertFrom-GzipString H4sIAAAAAAAACstIzcnJ5+Uqzy/KSeHlUuTlAgBLr/K2EQAAAA==

# Example 3: Demonstrates how `-NoNewLine` works

# New lines are preserved when the function receives an array of strings
$strings | ConvertTo-GzipString | ConvertFrom-GzipString
# When using the switch, all strings are concatenated
$strings | ConvertTo-GzipString -NoNewLine | ConvertFrom-GzipString

# Example 4: Create a Gzip compressed file from a string

# Demonstrates how `-Raw` works

'hello world!' | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip -Force

# Example 5: Append content to the previous Gzip file

# Demonstrates how `-Update` works
'this is new content...' | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip -Update

# Example 6: Expanding a Gzip file

# Output goes to the Success Stream when `-DestinationPath` is not bound
Expand-GzipArchive .\files\file.gzip

# Example 7: Replace the previous Gzip file with new content

# Demonstrates how `-Force` works
$lorem = Invoke-RestMethod loripsum.net/api/10/long/plaintext
$lorem | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip -Force

# Example 8: Expanding a Gzip file outputting to a file
Expand-GzipArchive .\files\file.gzip -DestinationPath .\files\file.txt

# Check Length Difference
Get-Item -Path .\files\file.gzip, .\files\file.txt |
    Select-Object Name, Length

# Check Integrity
$lorem | Set-Content .\files\strings.txt
Get-FileHash -Path .\files\file.txt, .\files\strings.txt -Algorithm MD5 |
    Select-Object Hash



# Example 9: Compressing multiple files into one Gzip file

# Due to the nature of Gzip without Tar, all files are merged into one file
0..10 | ForEach-Object {
    Invoke-RestMethod loripsum.net/api/10/long/plaintext -OutFile .\files\lorem$_.txt
}

(Get-Content .\files\lorem*.txt | Measure-Object Length -Sum).Sum / 1kb                                    # About 90kb
(Compress-GzipArchive .\files\lorem*.txt -DestinationPath .\files\mergedLorem.gzip -PassThru).Length / 1kb # About 30kb
(Expand-GzipArchive .\files\mergedLorem.gzip -Raw | Measure-Object Length -Sum).Sum / 1kb                       # About 90kb

# Example 10: Compressing the files content from previous example into one Gzip Base64 string

(Get-Content .\files\lorem*.txt | ConvertTo-GzipString).Length / 1kb                               # About 40kb
(Get-Content .\files\lorem*.txt | ConvertTo-GzipString | ConvertFrom-GzipString -Raw).Length / 1kb # About 90kb