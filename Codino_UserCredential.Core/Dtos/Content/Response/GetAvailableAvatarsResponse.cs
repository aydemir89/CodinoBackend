using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content.Response;

public class GetAvailableAvatarsResponse : ApiResponse
{
    public List<AvatarDto> Avatars { get; set; } = new List<AvatarDto>();
}
public class AvatarDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }
    public int ToyId { get; set; }
    public string ToyName { get; set; }
    public bool IsUnlocked { get; set; }
    public bool IsActive { get; set; }
    public int RequiredXp { get; set; }
}