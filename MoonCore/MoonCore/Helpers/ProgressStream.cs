namespace MoonCore.Helpers;

public class ProgressStream : Stream
{
    public override bool CanRead => InnerStream.CanRead;
    public override bool CanSeek => InnerStream.CanSeek;
    public override bool CanWrite => InnerStream.CanWrite;
    public override long Length => InnerStream.Length;

    public override long Position
    {
        get => InnerStream.Position;
        set => InnerStream.Position = value;
    }

    public override int WriteTimeout
    {
        get => InnerStream.WriteTimeout;
        set => InnerStream.WriteTimeout = value;
    }

    public override int ReadTimeout
    {
        get => InnerStream.ReadTimeout;
        set => InnerStream.ReadTimeout = value;
    }

    public override bool CanTimeout => InnerStream.CanTimeout;

    private readonly IProgress<long>? OnReadChanged;
    private readonly IProgress<long>? OnWriteChanged;
    private readonly Stream InnerStream;

    private long ReadCount;
    private long WriteCount;

    public ProgressStream(
        Stream innerStream,
        IProgress<long>? onReadChanged = null,
        IProgress<long>? onWriteChanged = null
    )
    {
        InnerStream = innerStream;
        OnReadChanged = onReadChanged;
        OnWriteChanged = onWriteChanged;
    }

    public override void Flush()
        => InnerStream.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken)
        => InnerStream.FlushAsync(cancellationToken);

    #region Read

    public override int Read(byte[] buffer, int offset, int count)
    {
        var read = InnerStream.Read(buffer, offset, count);

        if (OnReadChanged != null)
        {
            ReadCount += read;
            OnReadChanged.Report(ReadCount);
        }

        return read;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var read = await InnerStream.ReadAsync(buffer, offset, count, cancellationToken);

        if (OnReadChanged != null)
        {
            ReadCount += read;
            OnReadChanged.Report(ReadCount);
        }

        return read;
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var read = await InnerStream.ReadAsync(buffer, cancellationToken);

        if (OnReadChanged != null)
        {
            ReadCount += read;
            OnReadChanged.Report(ReadCount);
        }

        return read;
    }

    public override int Read(Span<byte> buffer)
    {
        var read = InnerStream.Read(buffer);

        if (OnReadChanged != null)
        {
            ReadCount += read;
            OnReadChanged.Report(ReadCount);
        }

        return read;
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return InnerStream.BeginRead(buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
        var read = InnerStream.EndRead(asyncResult);

        if (OnReadChanged != null)
        {
            ReadCount += read;
            OnReadChanged.Report(ReadCount);
        }

        return read;
    }

    public override int ReadByte()
    {
        if (OnReadChanged != null)
        {
            ReadCount =+ 1;
            OnReadChanged.Report(ReadCount);
        }
        
        return InnerStream.ReadByte();
    }

    #endregion

    public override long Seek(long offset, SeekOrigin origin)
        => InnerStream.Seek(offset, origin);

    public override void SetLength(long value)
        => InnerStream.SetLength(value);

    #region Write

    public override void Write(byte[] buffer, int offset, int count)
    {
        InnerStream.Write(buffer, offset, count);

        if (OnWriteChanged != null)
        {
            WriteCount += count;
            OnWriteChanged.Report(WriteCount);
        }
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var result = InnerStream.WriteAsync(buffer, offset, count, cancellationToken);

        if (OnWriteChanged != null)
        {
            WriteCount += count;
            OnWriteChanged.Report(WriteCount);
        }

        return result;
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        InnerStream.Write(buffer);

        if (OnWriteChanged != null)
        {
            WriteCount += buffer.Length;
            OnWriteChanged.Report(WriteCount);
        }
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var result = InnerStream.WriteAsync(buffer, cancellationToken);

        if (OnWriteChanged != null)
        {
            WriteCount += buffer.Length;
            OnWriteChanged.Report(WriteCount);
        }

        return result;
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback,
        object? state)
    {
        var result = InnerStream.BeginWrite(buffer, offset, count, callback, state);

        if (OnWriteChanged != null)
        {
            WriteCount += count;
            OnWriteChanged.Report(WriteCount);
        }

        return result;
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
        InnerStream.EndWrite(asyncResult);
    }

    public override void WriteByte(byte value)
    {
        if (OnWriteChanged != null)
        {
            WriteCount += 1;
            OnWriteChanged.Report(WriteCount);
        }
        
        InnerStream.WriteByte(value);
    }

    #endregion
}