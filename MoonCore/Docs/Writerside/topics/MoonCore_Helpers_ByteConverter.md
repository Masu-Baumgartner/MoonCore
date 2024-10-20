# ByteConverter

## Overview
The byte converter is a small utility helper allowing you to convert between byte sizes without calculating it yourself

## Examples

### Convert kilobytes to megabytes
<code-block lang="c#">
    var kiloBytes = 1024;
    var megaBytes = ByteConverter.FromKiloBytes(kiloBytes).MegaBytes;
</code-block>

## Reference
[Source Code](https://github.com/Masu-Baumgartner/MoonCore/blob/main/MoonCore/MoonCore/Helpers/ByteConverter.cs)