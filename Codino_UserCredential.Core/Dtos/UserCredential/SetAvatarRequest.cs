namespace Codino_UserCredential.Core.Dtos;

public class SetAvatarRequest 
{
    public int UserId { get; set; }
    public string AvatarUrl { get; set; }
}