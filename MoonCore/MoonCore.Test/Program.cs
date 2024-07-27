/*
 *using MoonCore.Helpers.Unix;
using MoonCore.Helpers.Unix.Extensions;

var unixFs = new UnixFileSystem("/var/lib/moonlight/volumes/2");

unixFs.ReadDir("/cache", out _).ThrowIfError();
unixFs.ReadDir("/cache", out _).ThrowIfError();
Console.WriteLine("Removing");
unixFs.RemoveAll("/cache/app").ThrowIfError();
 *
 */

using MoonCore.Helpers;
using MoonCore.Test;

//var class1 = new Class1()
//{
//    Field2 = "ara"
//};

var class2 = new Class2()
{
    Field1 = "owo",
    Field2 = "o"
};

var class1 = Mapper.Map<Class1>(class2, ignoreNullValues: true);

Console.WriteLine(class1.Field1 ?? "NULL");
Console.WriteLine(class1.Field2 ?? "NULL");