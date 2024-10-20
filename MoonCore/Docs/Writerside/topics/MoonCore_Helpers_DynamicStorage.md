# DynamicStorage

## Overview
The dynamic storage utility allows you to save data in a key value storage regardless of the type of the object.
When getting the value for a key you can specify the type the value should be casted to.

## Examples

### Convert kilobytes to megabytes
<code-block lang="c#">
    var kiloBytes = 1024;
    var megaBytes = ByteConverter.FromKiloBytes(kiloBytes).MegaBytes;
</code-block>

## Reference
[Source Code](https://github.com/Masu-Baumgartner/MoonCore/blob/main/MoonCore/MoonCore/Helpers/ByteConverter.cs)