using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content.Response;

public class UserToysResponse : ApiResponse
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public List<UserToyDto> Toys { get; set; } = new List<UserToyDto>();
}
public class UserToyDto
{
    public int Id { get; set; }
    public int ToyId { get; set; }
    public string ToyName { get; set; }
    public string ToyDescription { get; set; }
    public string ToyImageUrl { get; set; }
    public DateTime UnlockDate { get; set; }
    public List<ToyAvatarDto> AvailableAvatars { get; set; } = new List<ToyAvatarDto>();
}