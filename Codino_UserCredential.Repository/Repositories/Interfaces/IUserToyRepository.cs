using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models.Content;

namespace Codino_UserCredential.Repository.Repositories.Interfaces;

public interface IUserToyRepository : IStandartRepository<UserToy, CodinoDbContext ,int>
{
}