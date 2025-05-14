using System.Linq.Expressions;

namespace Codino_UserCredential.Repository.Repositories.Interfaces;

public interface ILoginUserLogRepository
{
    void Update(UserRepository entity);
    void DeleteRange(IEnumerable<UserRepository> entities);
    void SaveChanges();
}
