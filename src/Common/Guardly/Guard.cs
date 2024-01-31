namespace GlobalSpace.Common.Guardly
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;

    [DebuggerNonUserCode]
    public static class Guard
    {
        public static void Argument<T>(Expression<Func<T>> expression, params ArgumentAssessment<T>[] assessments)
        {
            if (assessments == null || assessments.Length == 0)
                return;
            Argument<T> obj = Guard.RetrieveArgument<T>(expression);
            if (obj == null)
                return;
            try
            {
                foreach (ArgumentAssessment<T> argumentAssessment in assessments)
                    argumentAssessment(obj, (string) null);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message, obj.Name, ex);
            }
        }

        public static void Argument<T>(Expression<Func<T>> expression, ArgumentAssessment<T> assessment, string message)
        {
            if (assessment == null)
                return;
            Argument<T> obj = Guard.RetrieveArgument<T>(expression);
            if (obj == null)
                return;
            try
            {
                assessment(obj, message);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message, obj.Name, ex);
            }
        }

        private static Argument<T> RetrieveArgument<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
                return (Argument<T>) null;
            MemberExpression memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                return (Argument<T>) null;
            MemberInfo member = memberExpression.Member;
            return new Argument<T>(member.GetHashCode(), expression.Compile(), member);
        }
    }
}
