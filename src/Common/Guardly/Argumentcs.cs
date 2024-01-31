
namespace GlobalSpace.Common.Guardly
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerNonUserCode]
    public sealed class Argument<T> : GuardBase<T>
    {
        private readonly MemberInfo _member;

        public string Name
        {
            get
            {
                return _member.Name;
            }
        }

        internal Argument(int hashCode, Func<T> getter, MemberInfo member)
            : base(hashCode, getter)
        {
            _member = member;
        }
    }
}
