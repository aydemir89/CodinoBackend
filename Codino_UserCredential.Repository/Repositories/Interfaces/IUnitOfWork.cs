using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Codino_UserCredential.Repository.Repositories.Interfaces;

public interface IUnitOfWork<TDBContext>
{
    void SaveChanges();
    void SaveChangesAsync();
    void BeginTransaction();
    void CommitTransaction();
    void RollBackTransaction();

    void JoinExistingTransaction(IUnitOfWork<TDBContext> unitOfWork);
    
    IDbContextTransaction currentTransaction { get; set; }
    void BeginTransaction(IsolationLevel level);
    int SetIsolationLevel(IsolationLevel level);
    void OpenConnection();
    void CloseConnection();
}