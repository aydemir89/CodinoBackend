namespace Codino_UserCredential.Core.Dtos;

using Newtonsoft.Json;

public class LoginRequest
{
    [JsonProperty(PropertyName = "username")]
    public string Username { get; set; }

    [JsonProperty(PropertyName = "email")]
    public string Email { get; set; }
    
    [JsonProperty(PropertyName = "password")]
    public string Password { get; set; }
    
    [JsonProperty(PropertyName = "channnel")]
    public short? Channel { get; set; }
    
    [JsonProperty(PropertyName = "isAdmin")]
    public bool IsAdmin { get; set; }
}
