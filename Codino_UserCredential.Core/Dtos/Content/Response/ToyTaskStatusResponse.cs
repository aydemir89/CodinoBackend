using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content.Response;

public class ToyTaskStatusResponse : ApiResponse
{
    public int ToyId { get; set; }
    public string ToyName { get; set; }
    public List<TaskStatusDto> Tasks { get; set; } = new List<TaskStatusDto>();
}

public class TaskStatusDto
{
    public int TaskId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletionDate { get; set; }
}