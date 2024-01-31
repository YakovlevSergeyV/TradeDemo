namespace Infrastructure.Common.Conversion
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public static class DataConversion
    {
        private static readonly Dictionary<Type, ITryParser> _Parsers;
        private static IFormatProvider _formatProvider;

        static DataConversion()
        {
            FormatProvider = CultureInfo.CurrentCulture;
            _Parsers = new Dictionary<Type, ITryParser>();
            AddParser<byte>((string input, out byte value) =>
            {
                byte value2;
                var result = byte.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser<byte?>((string input, out byte? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                byte value2;
                var result = byte.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser<DateTime>((string input, out DateTime value) =>
            {
                DateTime result;
                if (DateTime.TryParse(input, FormatProvider, DateTimeStyles.None, out result))
                {
                    value = result;
                    return true;
                }

                value = DateTime.MinValue;
                return false;
            });
            AddParser<DateTime?>((string input, out DateTime? value) =>
            {
                if (input == null)
                {
                    value = new DateTime?();
                    return true;
                }

                DateTime result;
                if (DateTime.TryParse(input, FormatProvider, DateTimeStyles.None, out result))
                {
                    value = result;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser<short>((string input, out short value) =>
            {
                short value2;
                var result = short.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser<short?>((string input, out short? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                short value2;
                var result = short.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser<ushort>((string input, out ushort value) =>
            {
                ushort value2;
                var result = ushort.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser<ushort?>((string input, out ushort? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                ushort value2;
                var result = ushort.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser<int>((string input, out int value) =>
            {
                int value2;
                var result = int.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser<int?>((string input, out int? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                int value2;
                var result = int.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser<uint>((string input, out uint value) =>
            {
                uint value2;
                var result = uint.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser<uint?>((string input, out uint? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                uint value2;
                var result = uint.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser<long>((string input, out long value) =>
            {
                long value2;
                var result = long.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser<long?>((string input, out long? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                long value2;
                var result = long.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser<ulong>((string input, out ulong value) =>
            {
                ulong value2;
                var result = ulong.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser<ulong?>((string input, out ulong? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                ulong value2;
                var result = ulong.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser<bool>((string input, out bool value) =>
            {
                if (input == "0")
                {
                    value = false;
                    return true;
                }

                if (input == "1")
                {
                    value = true;
                    return true;
                }

                bool result;
                if (bool.TryParse(input, out result))
                {
                    value = result;
                    return true;
                }

                value = false;
                return false;
            });
            AddParser<bool?>((string input, out bool? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                if (input == "0")
                {
                    value = false;
                    return true;
                }

                if (input == "1")
                {
                    value = true;
                    return true;
                }

                bool result;
                if (bool.TryParse(input, out result))
                {
                    value = result;
                    return true;
                }

                value = false;
                return false;
            });
            AddParser<float>((string input, out float value) =>
            {
                float value2;
                var result = float.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                // Попробуем объехать стандарты России
                input = input.Replace(',', '.');
                result = float.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser<float?>((string input, out float? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                float value2;
                var result = float.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                // Попробуем объехать стандарты России
                input = input.Replace(',', '.');
                result = float.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser<double>((string input, out double value) =>
            {
                double value2;
                var result = double.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                // Попробуем объехать стандарты России
                input = input.Replace(',', '.');
                result = double.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser<double?>((string input, out double? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                double value2;
                var result = double.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                // Попробуем объехать стандарты России
                input = input.Replace(',', '.');
                result = double.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser<decimal>((string input, out decimal value) =>
            {
                decimal value2;
                var result = decimal.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                // Попробуем объехать стандарты России
                input = input.Replace(',', '.');
                result = decimal.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser<decimal?>((string input, out decimal? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                decimal value2;
                var result = decimal.TryParse(input, NumberStyles.Number, FormatProvider, out value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                // Попробуем объехать стандарты России
                input = input.Replace(',', '.');
                result = decimal.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser((string input, out string value) =>
            {
                value = input;
                return true;
            });
            AddParser<Guid>(Guid.TryParse);
            AddParser<Guid?>((string input, out Guid? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                try
                {
                    value = Guid.Parse(input);
                    return true;
                }
                catch
                {
                    value = null;
                    return false;
                }
            });
            AddParser<TimeSpan>((string input, out TimeSpan value) =>
            {
                try
                {
                    value = TimeSpan.Parse(input, FormatProvider);
                    return true;
                }
                catch
                {
                    value = TimeSpan.FromSeconds(0);
                    return false;
                }
            });
            AddParser<TimeSpan?>((string input, out TimeSpan? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                try
                {
                    value = TimeSpan.Parse(input, FormatProvider);
                    return true;
                }
                catch
                {
                    value = null;
                    return false;
                }
            });
            AddParser<Version>(Version.TryParse);
        }

        public static IFormatProvider FormatProvider
        {
            get { return _formatProvider; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                _formatProvider = value;
            }
        }

        public static void AddParser<T>(TryParseMethod<T> parseMethod)
        {
            _Parsers[typeof(T)] = new TryParser<T>(parseMethod);
        }

        public static T Convert<T>(string input)
        {
            T answer;
            if (Convert(input, out answer))
                return answer;

            throw new InvalidOperationException(string.Format("Невозможно преобразовать значение '{0}' к типу '{1}'.", input, typeof(T).FullName));
        }

        public static bool Convert<T>(string input, out T value)
        {
            object parseResult;
            bool success = Convert(typeof(T), input, out parseResult);
            if (success)
                value = (T)parseResult;
            else
                value = default(T);
            return success;
        }

        public static bool Convert(Type type, string input, out object value)
        {
            ITryParser parser;
            if (_Parsers.TryGetValue(type, out parser))
                return parser.TryParse(input, out value);

            throw new NotSupportedException(string.Format("Данный тип '{0}' не поддерживается.", type.FullName));
        }

        public static bool GetEnumValue<T>(string input, out T value)
        {
            try
            {
                value = (T)Enum.Parse(typeof(T), input, true);
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
        }
    }
}
