namespace Codino_UserCredential.Core.Dtos.Content.Request;

public class TaskCreateRequest
{
    
    public int ToyId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ExpectedPattern { get; set; }
}