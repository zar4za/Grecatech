using System.Text.Json.Serialization;

namespace Grecatech.Steam.Models
{
    internal interface IResponse
    {
        public bool Success { get; }
    }
}
