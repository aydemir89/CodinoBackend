using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content;

public class UserProgressResponse : ApiResponse
{
    public List<BiomeProgress> BiomeProgresses { get; set; } = new List<BiomeProgress>();
}

public class BiomeProgress
{
    public int BiomeId { get; set; }
    public string BiomeName { get; set; }
    public int CompletedTaskCount { get; set; }
    public int TotalTaskCount { get; set; }
    public int CompletionRate { get; set; }
}