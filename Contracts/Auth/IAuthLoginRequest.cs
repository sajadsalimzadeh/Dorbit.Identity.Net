using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Dorbit.Identity.Contracts.Auth;

public interface IAuthLoginRequest
{
    [JsonIgnore]
    public string UserAgent { get; set; }
}