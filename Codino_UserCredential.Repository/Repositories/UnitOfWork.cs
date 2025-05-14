using System.Data;
using Codino_UserCredential.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Codino_UserCredential.Repository.Repositories;

public class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext> where TDbContext : DbContext
{
    public TDbContext CurrentDbContext { get; set; }
    public IDbContextTransaction CurrentTransaction { get; set; }

    public UnitOfWork(TDbContext context)
    {
        CurrentDbContext = context;
    }
    
    public void SaveChanges()
    {
        CurrentDbContext.SaveChanges();
    }

    public void SaveChangesAsync()
    {
        CurrentDbContext.SaveChangesAsync();
    }

    public void BeginTransaction()
    {
        if (CurrentTransaction != null)
        {
            return;
        }

        // Eğer bağlantı kapalıysa aç
        if (CurrentDbContext.Database.GetDbConnection().State != ConnectionState.Open)
        {
            CurrentDbContext.Database.GetDbConnection().Open();
        }
        
        CurrentTransaction = CurrentDbContext.Database.BeginTransaction();
    }

    public void CommitTransaction()
    {
        try
        {
            CurrentTransaction?.Commit();
        }
        catch
        {
            RollBackTransaction();
            throw;
        }
        finally
        {
            if (CurrentTransaction != null)
            {
                CurrentTransaction.Dispose();
                CurrentTransaction = null;
            }
        }
    }

    public void RollBackTransaction()
    {
        try
        {
            CurrentTransaction?.Rollback();
        }
        finally
        {
            if (CurrentTransaction != null)
            {
                CurrentTransaction.Dispose();
                CurrentTransaction = null;
            }
        }
    }

    public void JoinExistingTransaction(IUnitOfWork<TDbContext> unitOfWork)
    {
        if (unitOfWork.currentTransaction != null)
        {
            CurrentTransaction = unitOfWork.currentTransaction;
        }
    }

    public IDbContextTransaction currentTransaction { get; set; }
    
    public void BeginTransaction(IsolationLevel level)
    {
        if (CurrentTransaction != null)
        {
            return;
        }

        // Eğer bağlantı kapalıysa aç
        if (CurrentDbContext.Database.GetDbConnection().State != ConnectionState.Open)
        {
            CurrentDbContext.Database.GetDbConnection().Open();
        }
        
        CurrentTransaction = CurrentDbContext.Database.BeginTransaction(level);
        currentTransaction = CurrentTransaction;
    }

    public int SetIsolationLevel(IsolationLevel level)
    {
        if (CurrentDbContext.Database.GetDbConnection().State != ConnectionState.Open)
        {
            OpenConnection();
        }
        
        using var command = CurrentDbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = $"SET TRANSACTION ISOLATION LEVEL {GetIsolationLevelString(level)}";
        return command.ExecuteNonQuery();
    }

    private string GetIsolationLevelString(IsolationLevel level)
    {
        return level switch
        {
            IsolationLevel.ReadUncommitted => "READ UNCOMMITTED",
            IsolationLevel.ReadCommitted => "READ COMMITTED",
            IsolationLevel.RepeatableRead => "REPEATABLE READ",
            IsolationLevel.Serializable => "SERIALIZABLE",
            IsolationLevel.Snapshot => "SNAPSHOT",
            _ => "READ COMMITTED" // Varsayılan
        };
    }

    public void OpenConnection()
    {
        if (CurrentDbContext.Database.GetDbConnection().State != ConnectionState.Open)
        {
            CurrentDbContext.Database.GetDbConnection().Open();
        }
    }

    public void CloseConnection()
    {
        if (CurrentDbContext.Database.GetDbConnection().State == ConnectionState.Open)
        {
            CurrentDbContext.Database.GetDbConnection().Close();
        }
    }
}