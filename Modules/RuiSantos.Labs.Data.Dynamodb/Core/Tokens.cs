using System.IO.Compression;
using System.Text;

namespace RuiSantos.Labs.Data.Dynamodb.Core;

internal static class Tokens
{
    public static string? Encode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        using var stream = new MemoryStream();
        using var gzip = new GZipStream(stream, CompressionMode.Compress, true);
        using var writer = new StreamWriter(gzip, Encoding.UTF8);

        writer.Write(value);
        writer.Close();

        return Convert.ToBase64String(stream.ToArray());
    }

    public static string? Decode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var bytes = Convert.FromBase64String(value);

        using var stream = new MemoryStream(bytes);
        using var gzip = new GZipStream(stream, CompressionMode.Decompress);
        using var reader = new StreamReader(gzip, Encoding.UTF8);

        return reader.ReadToEnd();
    }
}