using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models.Content;

namespace Codino_UserCredential.Repository.Repositories.Interfaces;

public interface IToyActivationCodeRepository : IStandartRepository<ToyActivationCode, CodinoDbContext, int>
{
    Task<ToyActivationCode?> GetByCodeAsync(string activationCode);
    Task<bool> IsCodeUsedAsync(string activationCode);
    Task<IEnumerable<ToyActivationCode>> GetActivationCodesByToyIdAsync(int toyId);
}
