using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models;

namespace Codino_UserCredential.Repository.Repositories.Interfaces;

public interface IUserLoginRequestRepository : IStandartRepository<UserLoginRequest , CodinoDbContext, long>
{
}