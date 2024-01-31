
namespace Infrastructure.Common.Conversion
{
    public class TryParser<T>
        : ITryParser
    {
        private readonly TryParseMethod<T> _parsingMethod;

        public TryParser(TryParseMethod<T> parsingMethod)
        {
            _parsingMethod = parsingMethod;
        }

        public bool TryParse(string input, out object value)
        {
            T parsedOutput;
            bool success = _parsingMethod(input, out parsedOutput);
            value = parsedOutput;

            return success;
        }
    }
}
