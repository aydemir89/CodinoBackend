namespace Codino_UserCredential.Core.Dtos.Content.Request;

public class TaskSubmissionRequest
{
    public int UserId { get; set; }
    public int TaskId { get; set; }
    public string SubmittedCode { get; set; }
}