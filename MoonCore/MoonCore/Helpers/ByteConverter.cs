namespace MoonCore.Helpers;

public class ByteConverter
{
    public long Bytes { get; set; }
    public int ConversionNumber { get; private set; } = 1024;

    public long KiloBytes
    {
        get => Bytes / ConversionNumber;
        set => Bytes = value * ConversionNumber;
    }
    
    public long MegaBytes
    {
        get => KiloBytes / ConversionNumber;
        set => KiloBytes = value * ConversionNumber;
    }
    
    public long GigaBytes
    {
        get => MegaBytes / ConversionNumber;
        set => MegaBytes = value * ConversionNumber;
    }

    public static ByteConverter FromBytes(long bytes, int conversionNumber = 1024)
    {
        return new()
        {
            Bytes = bytes,
            ConversionNumber = conversionNumber
        };
    }
    
    public static ByteConverter FromKiloBytes(long kiloBytes, int conversionNumber = 1024)
    {
        return new()
        {
            KiloBytes = kiloBytes,
            ConversionNumber = conversionNumber
        };
    }
    
    public static ByteConverter FromMegaBytes(long megaBytes, int conversionNumber = 1024)
    {
        return new()
        {
            MegaBytes = megaBytes,
            ConversionNumber = conversionNumber
        };
    }
    
    public static ByteConverter FromGigaBytes(long gigaBytes, int conversionNumber = 1024)
    {
        return new()
        {
            GigaBytes = gigaBytes,
            ConversionNumber = conversionNumber
        };
    }
}