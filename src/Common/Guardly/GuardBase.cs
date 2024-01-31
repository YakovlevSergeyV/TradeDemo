
namespace GlobalSpace.Common.Guardly
{
    using System;
    using System.Diagnostics;

    [DebuggerNonUserCode]
    public abstract class GuardBase<T> : GuardBase
    {
        private readonly Func<T> _getter;

        public T Value
        {
            get
            {
                return _getter();
            }
        }

        internal GuardBase(int hashCode, Func<T> getter)
            : base(hashCode)
        {
            _getter = getter;
        }
    }

    [DebuggerNonUserCode]
    public abstract class GuardBase : IEquatable<GuardBase>
    {
        private readonly int _hashCode;

        internal GuardBase(int hashCode)
        {
            _hashCode = hashCode;
        }

        public bool Equals(GuardBase other)
        {
            if (object.ReferenceEquals((object)other, (object)null))
                return false;
            if (object.ReferenceEquals((object)other, (object)this))
                return true;
            if (GetType() != other.GetType())
                return false;
            return GetHashCode() == other.GetHashCode();
        }

        public override sealed bool Equals(object obj)
        {
            if (obj is GuardBase)
                return Equals(obj as GuardBase);
            return false;
        }

        public override sealed int GetHashCode()
        {
            return _hashCode;
        }
    }
}
