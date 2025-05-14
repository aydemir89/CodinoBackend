using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models.Content;
using Codino_UserCredential.Repository.Repositories.Interfaces;

namespace Codino_UserCredential.Repository.Repositories.Content;

public class ToyRepository : StandartRepository<Toy,CodinoDbContext, int> , IToyRepository
{
    public ToyRepository(CodinoDbContext context) : base(context)
    {
    }
}