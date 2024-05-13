using MoonCore.Helpers;
using MoonCore.Helpers.Unix;

Logger.Setup(isDebug: true);

var unixFs = new UnixFileSystem("/home/masu/chroot");

var error = unixFs.RemoveAll("x");

error.ThrowIfError();