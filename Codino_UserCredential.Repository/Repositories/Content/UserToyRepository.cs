using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models.Content;
using Codino_UserCredential.Repository.Repositories.Interfaces;

namespace Codino_UserCredential.Repository.Repositories.Content;

public class UserToyRepository : StandartRepository<UserToy, CodinoDbContext, int>, IUserToyRepository
{
    public UserToyRepository(CodinoDbContext context) : base(context)
    {
    }
}