using System.Reflection;
using Codino_UserCredential.Repository.Repositories.Interfaces;
using Codino_UserCredential.Repository.Utils;
using Microsoft.EntityFrameworkCore;

namespace Codino_UserCredential.Repository;

public class BaseContext : DbContext
{
    private Type modelTypeInfo;

    public BaseContext(DbContextOptions options) : base(options)
    {
        modelTypeInfo = typeof(BaseContext);
        ContextAttribute contextAttribute = GetType().GetCustomAttributes<ContextAttribute>().FirstOrDefault();
        if (contextAttribute != null)
        {
            modelTypeInfo = contextAttribute.DummyTable;
        }
    }

    public void CreateModel<T>(ModelBuilder modelBuilder, string tableName) where T : class
    {
        modelBuilder.Entity<T>().ToTable(tableName, "dbo");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = Assembly.GetAssembly(modelTypeInfo);

        if (assembly == null)
            throw new Exception("Assembly could not be loaded.");

        var types = assembly.GetTypes()
            .Where(f => f.IsClass && typeof(IBaseModel).IsAssignableFrom(f) && !f.FullName.Contains(".System.") && !f.FullName.Contains("Codino_UserCredential.Repository"))
            .ToList();

        var createModelMethod = this.GetType().GetMethod("CreateModel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var type in types)
        {
            createModelMethod.MakeGenericMethod(type).Invoke(this, new object[] { modelBuilder, type.Name });
        }
    }

    
}