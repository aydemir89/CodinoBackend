using Codino_UserCredential.Repository.Context;

namespace Codino_UserCredential.Repository.Repositories.Interfaces;

public interface IUserRepository : IStandartRepository<User, CodinoDbContext, int>
{
}