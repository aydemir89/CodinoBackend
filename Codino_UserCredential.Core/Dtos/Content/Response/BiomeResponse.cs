using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content;

public class BiomeResponse : ApiResponse
{
    public int BiomeId { get; set; }
    public string Name { get; set; }
    public string BackgroundImageUrl { get; set; }
    public List<ToyDto> Toys = new List<ToyDto>();
}

public class ToyDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string IconImageUrl { get; set; }
    public string Description { get; set; }
    public int TaskCount { get; set; }
}