namespace TradesReader.Api.Extensions
{
    using TradesReader.Api.Dto;

    public static class UrlsConfigVersionExtensions
    {
        public static string GetVersion1(this UrlsConfig urls)
        {
            return "/api/v1";
        }
    }
}
