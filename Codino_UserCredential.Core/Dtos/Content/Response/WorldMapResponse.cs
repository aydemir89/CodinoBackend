using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content;

public class WorldMapResponse : ApiResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string BackgroundImageUrl { get; set; }
}