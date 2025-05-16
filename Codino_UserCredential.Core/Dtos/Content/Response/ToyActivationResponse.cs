using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content.Response;

public class ToyActivationResponse : ApiResponse
{
    public int ToyId { get; set; }
    public string ToyName { get; set; }
    public string ToyImageUrl { get; set; }
    public string ActivationCode { get; set; }
    public bool IsNewlyActivated { get; set; }
    public List<ToyAvatarDto> UnlockedAvatars { get; set; } = new();
    
    public string Description { get; set; }
    public int Xp { get; set; } 
    public ToyTypeDto ToyType { get; set; }
    
    public int UserCurrentXp { get; set; }
    public int UserLevel { get; set; }
    public bool LevelledUp { get; set; }
}

public class ToyAvatarDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }
    public int RequiredToyLevel { get; set; }
    public int RequiredUserXp { get; set; }
}

public class ToyTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
}