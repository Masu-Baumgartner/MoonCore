using MoonCore.Helpers;

Logger.Setup(isDebug: true);

var chrootEnv = new ChrootFileSystem("/home/masu/chroot");

await chrootEnv.MoveDirectory("/test", "/x/config.json");