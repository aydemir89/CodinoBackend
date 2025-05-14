using System.Text.Json.Serialization;
using Codino_UserCredential.Repository.Repositories;
using Newtonsoft.Json;

namespace Codino.UserCredential.Core.DTOs;

public class LoginResponse : ApiResponse
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; } = "";
    
    [JsonProperty(PropertyName = "username")]
    public string Username { get; set; } = "";

    [JsonProperty(PropertyName = "password")]
    public string Password { get; set; }
    
    [JsonProperty(PropertyName = "level")]
    public int Level { get; set; }

    [JsonProperty(PropertyName = "xp")]
    public int Xp { get; set; }

    [JsonProperty(PropertyName = "xpForNextLevel")]
    public int XpForNextLevel { get; set; }

    [JsonProperty(PropertyName = "selectedAvatarUrl")]
    public string? SelectedAvatarUrl { get; set; }

    [JsonProperty(PropertyName = "availableAvatars")]
    public List<string> AvailableAvatars { get; set; } = new();
    
    [JsonProperty(PropertyName = "token")]
    public string Token { get; set; } = "";
}