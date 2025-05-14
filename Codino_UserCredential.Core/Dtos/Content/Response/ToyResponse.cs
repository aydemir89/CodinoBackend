using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content;

public class ToyResponse : ApiResponse
{
    public int ToyId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconImageUrl { get; set; }
}