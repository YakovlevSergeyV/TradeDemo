
namespace Infrastructure.Common.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ModifyTimeFrameAttribute : Attribute
    {
        public bool Modify { get; set; }
    }
}
