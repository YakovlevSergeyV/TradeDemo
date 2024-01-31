
namespace Infrastructure.Common.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class StepValueAttribute : Attribute
    {
        public double Value { get; set; }
    }
}
