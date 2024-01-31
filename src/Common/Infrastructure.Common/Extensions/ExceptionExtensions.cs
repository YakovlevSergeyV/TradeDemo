
namespace Infrastructure.Common.Extensions
{
    using System;
    using System.ComponentModel;
    using System.Text;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExceptionExtensions
    {
        public static string MessageAggregateException(this Exception e)
        {
            return GetInnerException(e).Message;
        }

        private static Exception GetInnerException(Exception e)
        {
            return e is AggregateException ex ? GetInnerException(ex.InnerException) : e;
        }
        
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string CleanException(this Exception exception)
        {
            if (exception == null) throw new ArgumentNullException("exception");

            var stringBuilder = new StringBuilder(string.Empty);

            stringBuilder.AppendLine(CleanMessage(exception.Message));

            var innerException = exception.InnerException;
            while (innerException != null)
            {
                stringBuilder.AppendLine(CleanMessage(innerException.Message));
                innerException = innerException.InnerException;
            }

            return stringBuilder.ToString();
        }

        private static string CleanMessage(string message)
        {
            return String.IsNullOrEmpty(message)
                ? ""
                : message.Replace('\r', ' ').Replace('\n', ' ');
        }
    }
}
