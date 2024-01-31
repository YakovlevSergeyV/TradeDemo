namespace Infrastructure.Common.Conversion
{
    public interface ITryParser
    {
        bool TryParse(string input, out object value);
    }
}
