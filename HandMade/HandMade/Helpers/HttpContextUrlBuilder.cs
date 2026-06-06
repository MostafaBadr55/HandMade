using HandMade.Application.Interfaces;

namespace HandMade.Helpers
{
    public class HttpContextUrlBuilder : IUrlBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextUrlBuilder(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string BuildAbsoluteUrl(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return string.Empty;

            var request = _httpContextAccessor.HttpContext?.Request;

            if (request is null)
                return relativePath;

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{relativePath.TrimStart('/')}";
        }
    }
}
