namespace MoonCore.Blazor.FlyonUi.Files.View;

public static class FileIconHelper
{
    /// <summary>
    /// Returns a lucide icon matching the type of the file by extension matching
    /// </summary>
    /// <param name="fileName">File name to get the icon for</param>
    /// <returns></returns>
    public static string GetByName(string fileName)
    {
        string identifier;

        if (fileName.Contains('.'))
            identifier = Path.GetExtension(fileName).Remove(0, 1);
        else
            identifier = fileName;
        
        return identifier switch
        {
            // Executables & binaries
            "exe" => "icon-file-digit",
            "dll" => "icon-file-digit",
            "so" => "icon-file-digit",
            "bin" => "icon-file-digit",
            "dat" => "icon-file-digit",
            "elf" => "icon-file-digit",
            "msi" => "icon-file-digit",
            "deb" => "icon-file-digit",
            "rpm" => "icon-file-digit",
            "pkg" => "icon-file-digit",
            "dmg" => "icon-file-digit",

            // Scripts
            "sh" => "icon-file-terminal",
            "bat" => "icon-file-terminal",
            "cmd" => "icon-file-terminal",
            "com" => "icon-file-digit",
            "jar" => "icon-file-archive",
            "py" => "icon-file-code",
            "pyc" => "icon-file-code",
            "pyw" => "icon-file-code",
            "js" => "icon-file-code",
            "ts" => "icon-file-code",
            "java" => "icon-file-code",
            "class" => "icon-file-code",

            // C/C++/C# and related
            "cpp" => "icon-file-code",
            "cc" => "icon-file-code",
            "cxx" => "icon-file-code",
            "c" => "icon-file-code",
            "h" => "icon-file-code",
            "hpp" => "icon-file-code",
            "cs" => "icon-file-code",

            // Other programming languages
            "go" => "icon-file-code",
            "rs" => "icon-file-code",
            "swift" => "icon-file-code",
            "kt" => "icon-file-code",
            "kts" => "icon-file-code",
            "rb" => "icon-file-code",
            "php" => "icon-file-code",
            "pl" => "icon-file-code",
            "pm" => "icon-file-code",
            "lua" => "icon-file-code",
            "sql" => "icon-database",
            "r" => "icon-file-code",
            "m" => "icon-file-code",
            "scala" => "icon-file-code",
            "vb" => "icon-file-code",
            "groovy" => "icon-file-code",

            // Markup & config
            "yaml" or "yml" => "icon-file-code",
            "json" => "icon-file-json",
            "xml" => "icon-file-code",
            "ini" => "icon-file-code",
            "cfg" => "icon-file-code",
            "conf" => "icon-file-code",
            "toml" => "icon-file-code",
            "env" => "icon-file-code",
            "properties" => "icon-file-code",

            // Text & documents
            "log" => "icon-file-text",
            "txt" => "icon-file-text",
            "md" => "icon-file-text",
            "csv" => "icon-file-spreadsheet",
            "tsv" => "icon-file-spreadsheet",
            "html" or "htm" => "icon-file-code",
            "css" => "icon-file-code",
            "scss" or "sass" => "icon-file-code",
            "vue" => "icon-file-code",
            "jsx" or "tsx" => "icon-file-code",

            // Security & certs
            "gpg" => "icon-file-key",
            "pem" => "icon-file-key",
            "crt" => "icon-file-key",
            "cer" => "icon-file-key",
            "key" => "icon-file-key",
            "pfx" => "icon-file-key",
            "p12" => "icon-file-key",

            // Archives & images
            "zip" => "icon-file-archive",
            "tar" => "icon-file-archive",
            "gz" => "icon-file-archive",
            "bz2" => "icon-file-archive",
            "xz" => "icon-file-archive",
            "7z" => "icon-file-archive",
            "rar" => "icon-file-archive",
            "iso" => "icon-file-archive",
            "img" => "icon-file-image",
            "vhd" => "icon-file-archive",
            "vmdk" => "icon-file-archive",

            // Minecraft related files
            "mcworld" => "icon-file-archive",
            "mcfunction" => "icon-file-code",
            "nbt" => "icon-file-code",
            "mca" => "icon-file-code",

            _ => "icon-file", // fallback default icon
        };
    }
}