namespace GlobalSpace.Common.Guardly
{
    using System.Diagnostics;
    using System.Text;

    [DebuggerNonUserCode]
    internal static class Reason
    {
        public static string Pluralize(this int count, string singular, string plural)
        {
            if (count != 1)
                return plural;
            return singular;
        }

        public static StringBuilder Compose(string baseMessage, string extendedMessage)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (extendedMessage != null && extendedMessage.Trim() != string.Empty)
            {
                stringBuilder.AppendFormat("{0}.", (object)extendedMessage);
                stringBuilder.AppendLine();
            }
            stringBuilder.AppendFormat("{0}.", (object)baseMessage);
            return stringBuilder;
        }
    }
}
