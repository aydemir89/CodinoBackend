using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models.Content;
using Codino_UserCredential.Repository.Repositories.Interfaces;

namespace Codino_UserCredential.Repository.Repositories.Content;

public class WorldMapRepository : StandartRepository<WorldMap , CodinoDbContext, int>, IWorldMapRepository
{
    public WorldMapRepository(CodinoDbContext context) : base(context)
    {
    }
}