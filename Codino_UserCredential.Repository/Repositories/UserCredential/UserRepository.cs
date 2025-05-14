using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Repositories.Interfaces;

namespace Codino_UserCredential.Repository.Repositories;

public class UserRepository : StandartRepository<User, CodinoDbContext, int>, IUserRepository
{
    public UserRepository(CodinoDbContext context) : base(context)
    {
    }
}