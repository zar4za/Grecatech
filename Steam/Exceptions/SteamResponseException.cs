namespace Grecatech.Steam.Exceptions
{
    internal class SteamResponseException : HttpRequestException
    {
        public string? JsonKeyName { get; }
        public SteamResponseException(string? message) : base(message)
        {
            JsonKeyName = null;
        }
    }
}
