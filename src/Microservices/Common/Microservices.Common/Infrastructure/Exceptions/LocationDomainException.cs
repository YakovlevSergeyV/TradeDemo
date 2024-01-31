namespace Microservices.Common.Infrastructure.Exceptions
{
    using System;

    public class LocationDomainException : Exception
    {
        public LocationDomainException()
        { }

        public LocationDomainException(string message)
            : base(message)
        { }

        public LocationDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
