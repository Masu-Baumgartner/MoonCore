namespace MoonCore.Helpers;

/// <summary>
/// Represents a utility for converting between bytes and larger storage units
/// (KB, MB, GB, TB, PB) with configurable conversion base (1000 or 1024).
/// </summary>
public class ByteConverter : IComparable<ByteConverter>, IEquatable<ByteConverter>
{
    /// <summary>
    /// Gets the number of bytes represented by this instance.
    /// </summary>
    public long Bytes { get; private set; }

    /// <summary>
    /// Gets the conversion number used for unit conversion.
    /// Defaults to 1024 (binary units). Set to 1000 for decimal units.
    /// </summary>
    public int ConversionNumber { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ByteConverter"/> class.
    /// </summary>
    /// <param name="bytes">The number of bytes.</param>
    /// <param name="conversionNumber">The conversion base (1000 for decimal, 1024 for binary).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if bytes is negative or conversionNumber is not positive.</exception>
    public ByteConverter(long bytes, int conversionNumber = 1024)
    {
        if (bytes < 0) throw new ArgumentOutOfRangeException(nameof(bytes), "Bytes cannot be negative.");
        if (conversionNumber <= 0) throw new ArgumentOutOfRangeException(nameof(conversionNumber), "Conversion number must be positive.");

        Bytes = bytes;
        ConversionNumber = conversionNumber;
    }

    /// <summary>
    /// Gets or sets the size in kilobytes.
    /// </summary>
    public double KiloBytes
    {
        get => ConvertFrom(Bytes, 1);
        set => Bytes = ConvertTo(value, 1);
    }

    /// <summary>
    /// Gets or sets the size in megabytes.
    /// </summary>
    public double MegaBytes
    {
        get => ConvertFrom(Bytes, 2);
        set => Bytes = ConvertTo(value, 2);
    }

    /// <summary>
    /// Gets or sets the size in gigabytes.
    /// </summary>
    public double GigaBytes
    {
        get => ConvertFrom(Bytes, 3);
        set => Bytes = ConvertTo(value, 3);
    }

    /// <summary>
    /// Gets or sets the size in terabytes.
    /// </summary>
    public double TeraBytes
    {
        get => ConvertFrom(Bytes, 4);
        set => Bytes = ConvertTo(value, 4);
    }

    /// <summary>
    /// Gets or sets the size in petabytes.
    /// </summary>
    public double PetaBytes
    {
        get => ConvertFrom(Bytes, 5);
        set => Bytes = ConvertTo(value, 5);
    }

    /// <summary>
    /// Converts bytes into a higher unit using the conversion base.
    /// </summary>
    private double ConvertFrom(long bytes, int power) =>
        bytes / Math.Pow(ConversionNumber, power);

    /// <summary>
    /// Converts a higher unit into bytes using the conversion base.
    /// </summary>
    private long ConvertTo(double value, int power) =>
        (long)(value * Math.Pow(ConversionNumber, power));
    

    /// <summary>
    /// Creates a new instance from a byte value.
    /// </summary>
    public static ByteConverter FromBytes(long bytes, int conversionNumber = 1024) =>
        new(bytes, conversionNumber);

    /// <summary>
    /// Creates a new instance from a kilobyte value.
    /// </summary>
    public static ByteConverter FromKiloBytes(double kiloBytes, int conversionNumber = 1024) =>
        new((long)(kiloBytes * Math.Pow(conversionNumber, 1)), conversionNumber);

    /// <summary>
    /// Creates a new instance from a megabyte value.
    /// </summary>
    public static ByteConverter FromMegaBytes(double megaBytes, int conversionNumber = 1024) =>
        new((long)(megaBytes * Math.Pow(conversionNumber, 2)), conversionNumber);

    /// <summary>
    /// Creates a new instance from a gigabyte value.
    /// </summary>
    public static ByteConverter FromGigaBytes(double gigaBytes, int conversionNumber = 1024) =>
        new((long)(gigaBytes * Math.Pow(conversionNumber, 3)), conversionNumber);

    /// <summary>
    /// Creates a new instance from a terabyte value.
    /// </summary>
    public static ByteConverter FromTeraBytes(double teraBytes, int conversionNumber = 1024) =>
        new((long)(teraBytes * Math.Pow(conversionNumber, 4)), conversionNumber);


    /// <summary>
    /// Returns a human-readable string representation of this instance, 
    /// choosing the largest appropriate unit (B, KB, MB, GB, TB, PB).
    /// </summary>
    public override string ToString()
    {
        if (PetaBytes >= 1) return $"{PetaBytes:F2} PB";
        if (TeraBytes >= 1) return $"{TeraBytes:F2} TB";
        if (GigaBytes >= 1) return $"{GigaBytes:F2} GB";
        if (MegaBytes >= 1) return $"{MegaBytes:F2} MB";
        if (KiloBytes >= 1) return $"{KiloBytes:F2} KB";
        return $"{Bytes} B";
    }

    // Equality and Comparison

    /// <inheritdoc/>
    public bool Equals(ByteConverter? other) => other is not null && Bytes == other.Bytes;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ByteConverter other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => Bytes.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(ByteConverter? other)
    {
        if (other is null) return 1;
        return Bytes.CompareTo(other.Bytes);
    }

    // Operator Overloads

    /// <summary>Determines whether two <see cref="ByteConverter"/> instances are equal.</summary>
    public static bool operator ==(ByteConverter a, ByteConverter b) => a.Equals(b);

    /// <summary>Determines whether two <see cref="ByteConverter"/> instances are not equal.</summary>
    public static bool operator !=(ByteConverter a, ByteConverter b) => !a.Equals(b);

    /// <summary>Determines whether one <see cref="ByteConverter"/> instance is less than another.</summary>
    public static bool operator <(ByteConverter a, ByteConverter b) => a.Bytes < b.Bytes;

    /// <summary>Determines whether one <see cref="ByteConverter"/> instance is greater than another.</summary>
    public static bool operator >(ByteConverter a, ByteConverter b) => a.Bytes > b.Bytes;

    /// <summary>Determines whether one <see cref="ByteConverter"/> instance is less than or equal to another.</summary>
    public static bool operator <=(ByteConverter a, ByteConverter b) => a.Bytes <= b.Bytes;

    /// <summary>Determines whether one <see cref="ByteConverter"/> instance is greater than or equal to another.</summary>
    public static bool operator >=(ByteConverter a, ByteConverter b) => a.Bytes >= b.Bytes;

    /// <summary>Adds two <see cref="ByteConverter"/> instances together.</summary>
    public static ByteConverter operator +(ByteConverter a, ByteConverter b) =>
        new(a.Bytes + b.Bytes, a.ConversionNumber);

    /// <summary>Subtracts one <see cref="ByteConverter"/> instance from another.</summary>
    public static ByteConverter operator -(ByteConverter a, ByteConverter b) =>
        new(Math.Max(0, a.Bytes - b.Bytes), a.ConversionNumber);
}