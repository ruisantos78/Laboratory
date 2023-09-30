using Sqids;

namespace RuiSantos.Labs.GraphQL.Services;

internal interface ISecurity {
    string Encode(Guid guid);
    Guid Decode(string? id);
}

internal class Security: ISecurity
{
    private readonly SqidsEncoder<int> _sqids = new();

    public string Encode(Guid guid)
    {
        var array = Array.ConvertAll(guid.ToByteArray(), Convert.ToInt32);
        return _sqids.Encode(array);
    }

    public Guid Decode(string? id)
    {
        if (id == null) return Guid.Empty;

        var array = _sqids.Decode(id).ToArray();
        var bytes = Array.ConvertAll(array, Convert.ToByte);
        return new Guid(bytes);
    }
}

