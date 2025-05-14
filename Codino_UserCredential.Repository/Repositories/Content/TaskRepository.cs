using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models.Content;
using Codino_UserCredential.Repository.Repositories.Interfaces;

namespace Codino_UserCredential.Repository.Repositories.Content;

public class TaskRepository : StandartRepository<ProgrammingTask,CodinoDbContext,int>, ITaskRepository
{
    public TaskRepository(CodinoDbContext context) : base(context)
    {
    }
}