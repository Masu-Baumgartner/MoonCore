namespace MoonCoreUI.Models;

public class MoonCoreUiConfiguration
{
    public string AlertJavascriptPrefix { get; set; } = "mooncore";
    public string ToastJavascriptPrefix { get; set; } = "mooncore";
    public string ClipboardJavascriptPrefix { get; set; } = "mooncore";
    public string ModalJavascriptPrefix { get; set; } = "mooncore";
    public string FileDownloadJavascriptPrefix { get; set; } = "mooncore";

    public bool UseSkeletonInLazyLoader { get; set; } = false;
    public int LazyLoaderSkeletonHeight { get; set; } = 20;
}