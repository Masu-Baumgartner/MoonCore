using MoonCore.Helpers;

Logger.Setup(isDebug: true);

var chrootEnv = new ChrootFileSystem("/home/masu/chroot");

foreach (var listDirectory in chrootEnv.ListDirectories("/x/x"))
{
    Console.WriteLine(listDirectory.Name);
}