using Mono.Unix.Native;
using MoonCore.Helpers;
using MoonCore.Unix.SecureFs;

var secureFs = new SecureFileSystem(
    PathBuilder.Dir(Directory.GetCurrentDirectory(), "testFs")
);

secureFs.RemoveAll("c");