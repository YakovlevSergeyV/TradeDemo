namespace Infrastructure.Common.Conversion
{
    public delegate bool TryParseMethod<T>(string input, out T value);
}
