using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.UserCredential;

public class GetAvatarResponse : ApiResponse
{
    public string AvatarUrl { get; set; } = "";
    public int AvatarId { get; set; } = 0;
    public string AvatarName { get; set; } = "Default";
    public int ToyId { get; set; } = 0;
    public string ToyName { get; set; } = "";
    public int RequiredXp { get; set; } = 0;
    public bool IsDefaultAvatar { get; set; } = true;
}