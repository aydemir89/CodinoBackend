using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models.Content;
using Codino_UserCredential.Repository.Repositories.Content;

namespace Codino_UserCredential.Repository.Repositories.Interfaces;

public interface ITaskSubmissionRepository : IStandartRepository<TaskSubmission, CodinoDbContext, int>;
