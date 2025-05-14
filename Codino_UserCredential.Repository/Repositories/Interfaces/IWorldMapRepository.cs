using Codino_UserCredential.Core.Dtos.Content;
using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models.Content;

namespace Codino_UserCredential.Repository.Repositories.Interfaces;

public interface IWorldMapRepository : IStandartRepository<WorldMap,CodinoDbContext, int>;