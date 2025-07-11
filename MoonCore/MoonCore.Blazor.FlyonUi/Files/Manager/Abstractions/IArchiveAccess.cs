﻿namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IArchiveAccess
{
    public ArchiveFormat[] ArchiveFormats { get; }

    public Task Archive(string path, ArchiveFormat format, string archiveRootPath, FsEntry[] files,
        Func<int, Task>? onProgress = null);

    public Task Unarchive(string path, ArchiveFormat format, string archiveRootPath,
        Func<int, Task>? onProgress = null);
}