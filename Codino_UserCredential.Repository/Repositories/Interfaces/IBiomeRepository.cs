using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models.Content;

namespace Codino_UserCredential.Repository.Repositories.Interfaces;

public interface IBiomeRepository : IStandartRepository<Biome, CodinoDbContext, int>;
