using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content.Response;

public class ToyActivationDetailsResponse : ApiResponse
{
    public int Id { get; set; }
    public string ActivationCode { get; set; }
    public string QrCodeImageUrl { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    
    public int ToyId { get; set; }
    public string ToyName { get; set; }
    public string ToyDescription { get; set; }
    public string ToyImageUrl { get; set; }
    public string ToyType { get; set; }
    
    public bool IsActivated { get; set; }
    public DateTime? ActivationDate { get; set; }
    
    public UserActivationInfoDto UserInfo { get; set; }
    
    public List<ToyAvatarDto> Avatars { get; set; } = new();
}

public class UserActivationInfoDto
{
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime RegistrationDate { get; set; }
    public int Level { get; set; }
    public int Xp { get; set; }
    public string CurrentAvatarUrl { get; set; }
}