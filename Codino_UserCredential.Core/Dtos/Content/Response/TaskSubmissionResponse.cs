using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content.Response;

public class TaskSubmissionResponse : ApiResponse
{
    public bool IsCorrect { get; set; }
}