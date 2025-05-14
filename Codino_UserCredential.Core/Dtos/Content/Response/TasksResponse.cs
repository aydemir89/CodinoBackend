using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content;

public class TasksResponse : ApiResponse
{
    public int ToyId { get; set; }
    public string ToyName { get; set; }
    public List<TaskDto> Tasks = new List<TaskDto>();
}

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}