using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models;
using Codino_UserCredential.Repository.Models.Content;
using Codino_UserCredential.Repository.Repositories.Interfaces;

namespace Codino_UserCredential.Repository.Repositories.Content;

public class TaskSubmissionRepository : StandartRepository<TaskSubmission, CodinoDbContext, int> , ITaskSubmissionRepository
{
    public TaskSubmissionRepository(CodinoDbContext context) : base(context)
    {
    }
}