namespace Codino_UserCredential.Core.Dtos;

public class UserRegisterRequestDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime? Birthdate { get; set; }
}