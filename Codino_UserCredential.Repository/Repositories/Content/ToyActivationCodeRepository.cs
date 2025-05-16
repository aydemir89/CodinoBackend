using Codino_UserCredential.Core.Enums;
using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models.Content;
using Codino_UserCredential.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Codino_UserCredential.Repository.Repositories.Content;

public class ToyActivationCodeRepository : StandartRepository<ToyActivationCode, CodinoDbContext, int>, IToyActivationCodeRepository
{
    private readonly CodinoDbContext _context;

    public ToyActivationCodeRepository(CodinoDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ToyActivationCode?> GetByCodeAsync(string activationCode)
    {
        return await _context.Set<ToyActivationCode>()
            .Where(tac => tac.ActivationCode == activationCode && tac.StatusId == (Status)Core.Enums.Status.Valid)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsCodeUsedAsync(string activationCode)
    {
        return await _context.Set<ToyActivationCode>()
            .AnyAsync(tac => tac.ActivationCode == activationCode && tac.IsActivated);
    }

    public async Task<IEnumerable<ToyActivationCode>> GetActivationCodesByToyIdAsync(int toyId)
    {
        return await _context.Set<ToyActivationCode>()
            .Where(code => code.ToyId == toyId)
            .ToListAsync();
    }
}