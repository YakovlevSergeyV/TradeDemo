namespace Microservices.Common.Info
{
    using System.Collections.Generic;

    public static class SwaggerInfo
    {
        static SwaggerInfo()
        {
            Doc = new SwaggerDocInfo();
            SecurityScopes= new Dictionary<string, string>();
            Endpoint = new SwaggerEndpointInfo();
        }

        public static SwaggerDocInfo Doc { get; private set; }

        public static Dictionary<string, string> SecurityScopes { get; private set; }

        public static string FilterAppKeyName { get; set; }

        public static SwaggerEndpointInfo Endpoint { get; private set; }
    }
}
