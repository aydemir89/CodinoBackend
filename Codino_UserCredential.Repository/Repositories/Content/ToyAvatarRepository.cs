using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models.Content;
using Codino_UserCredential.Repository.Repositories.Interfaces;

namespace Codino_UserCredential.Repository.Repositories.Content;

public class ToyAvatarRepository : StandartRepository<ToyAvatar, CodinoDbContext, int>, IToyAvatarRepository
{
    public ToyAvatarRepository(CodinoDbContext context) : base(context)
    {
    }
}