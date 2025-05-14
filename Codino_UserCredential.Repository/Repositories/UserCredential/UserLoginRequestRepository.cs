using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models;
using Codino_UserCredential.Repository.Repositories.Interfaces;

namespace Codino_UserCredential.Repository.Repositories
{
    public class UserLoginRequestRepository : StandartRepository<UserLoginRequest, CodinoDbContext, long>, IUserLoginRequestRepository
    {
        public UserLoginRequestRepository(CodinoDbContext context) : base(context)
        {
        }
    }
}