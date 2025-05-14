using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content.Response;

public class TaskSubmissionsResponse : ApiResponse
{
    public int TaskId { get; set; }
    public string TaskTitle { get; set; }
    public List<TaskSubmissionDto> Submissions { get; set; } = new List<TaskSubmissionDto>();
}

public class TaskSubmissionDto
{
    public int Id { get; set; }
    public string SubmittedCode { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime Timestamp { get; set; }
}