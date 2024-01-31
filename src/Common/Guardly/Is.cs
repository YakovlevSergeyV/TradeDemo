
namespace GlobalSpace.Common.Guardly
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    [DebuggerNonUserCode]
    public static class Is
    {
        public static void NotNull<T>(Argument<T> argument, string message) where T : class
        {
            if (object.ReferenceEquals((object)argument.Value, (object)null))
            {
                StringBuilder stringBuilder = Reason.Compose("Provided parameter should not be null", message);
                throw new ArgumentNullException(argument.Name, stringBuilder.ToString());
            }
        }

        public static void NotNullOrEmpty(Argument<string> argument, string message)
        {
            string str = argument.Value;
            if (str == null)
            {
                StringBuilder stringBuilder = Reason.Compose("Provided string should not be null", message);
                throw new ArgumentNullException(argument.Name, stringBuilder.ToString());
            }
            if (str == string.Empty)
            {
                StringBuilder stringBuilder = Reason.Compose("Provided string should not be empty", message);
                throw new ArgumentOutOfRangeException(argument.Name, (object)str, stringBuilder.ToString());
            }
        }

        public static void NotNullOrWhiteSpace(Argument<string> argument, string message)
        {
            string str = argument.Value;
            if (str == null)
            {
                StringBuilder stringBuilder = Reason.Compose("Provided string should not be null", message);
                throw new ArgumentNullException(argument.Name, stringBuilder.ToString());
            }
            if (str.Trim() == string.Empty)
            {
                StringBuilder stringBuilder = Reason.Compose("Provided string should not be empty or white space", message);
                throw new ArgumentOutOfRangeException(argument.Name, (object)str, stringBuilder.ToString());
            }
        }

        public static ArgumentAssessment<T> In<T>(Func<IEnumerable<T>> expression)
        {
            return (ArgumentAssessment<T>)((argument, message) =>
            {
                T obj = argument.Value;
                if (!Enumerable.Contains<T>(expression(), obj))
                {
                    string message1 = Reason.Compose("Provided parameter should be present in internal enumeration", message).ToString();
                    throw new ArgumentOutOfRangeException(argument.Name, (object)obj, message1);
                }
            });
        }

        public static ArgumentAssessment<T> NotIn<T>(Func<IEnumerable<T>> expression)
        {
            return (ArgumentAssessment<T>)((argument, message) =>
            {
                T obj = argument.Value;
                if (Enumerable.Contains<T>(expression(), obj))
                {
                    string message1 = Reason.Compose("Provided parameter should not be present in internal enumeration", message).ToString();
                    throw new ArgumentOutOfRangeException(argument.Name, (object)obj, message1);
                }
            });
        }
    }
}
