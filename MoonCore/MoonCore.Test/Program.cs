using MoonCore.Helpers.Unix;
using MoonCore.Helpers.Unix.Extensions;

var unixFs = new UnixFileSystem("/var/lib/moonlight/volumes/2");

unixFs.ReadDir("/cache", out _).ThrowIfError();
unixFs.ReadDir("/cache", out _).ThrowIfError();
Console.WriteLine("Removing");
unixFs.RemoveAll("/cache/app").ThrowIfError();