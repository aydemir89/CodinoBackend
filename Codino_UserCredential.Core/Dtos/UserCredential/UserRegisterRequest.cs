namespace Codino_UserCredential.Core.Dtos;

public class UserRegisterRequest
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Cellphone { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string RePassword { get; set; }
    
    public string TCKN { get; set; } 
    public bool IsChannelFromMobil { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool AllowKVKK { get; set; }
}