using AgentsDataView.Entities;
using AgentsDataView.Common.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AgentsDataView.Data
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var assembly = typeof(IEntity).Assembly;
           // modelBuilder.RegisterAllEntities<IEntity>(assembly);
            var types = modelBuilder.Model.GetEntityTypes().Select(t => t.ClrType).ToHashSet();
            modelBuilder.ApplyConfigurationsFromAssembly(assembly/*, t => t.GetInterfaces().Any(i => i.IsGenericType &&
                                                                                            i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>) &&
                                                                                            types.Contains(i.GenericTypeArguments[0]))*/);
            //modelBuilder.AddRestrictDeleteBehaviorConvention();
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            _cleanString();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _cleanString();
            return base.SaveChangesAsync(cancellationToken);
        }
        private void _cleanString()
        {
            var changedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            foreach (var changed in changedEntities)
            {
                if (changed.Entity == null)
                {
                    continue;
                }
                var properties = changed.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));
                foreach (var property in properties)
                {
                    string propName = property.Name;
                    string? val = property.GetValue(changed.Entity, null) as string;
                    if (string.IsNullOrWhiteSpace(val))
                    {
                        continue;
                    }
                    var newVal = val.Fa2En().FixPersianChars();
                    if (newVal == val)
                    {
                        continue;
                    }
                    property.SetValue(changed.Entity, newVal, null);
                }

            }
        }

    }
}
