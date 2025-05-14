using System.Linq.Expressions;
using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Repositories.Interfaces;

namespace Codino_UserCredential.Repository.Repositories;

public class LoginUserLogRepository : ILoginUserLogRepository
{
    private readonly CodinoDbContext _codinoDbContext;
    public LoginUserLogRepository(CodinoDbContext context)
    {
        _codinoDbContext = context;
    }
    //

    public void Update(UserRepository entity)
    {
        _codinoDbContext.Update(entity);
    }

    public void DeleteRange(IEnumerable<UserRepository> entities)
    {
        _codinoDbContext.RemoveRange(entities);
    }

    public void SaveChanges()
    {
        _codinoDbContext.SaveChanges();
    }
}