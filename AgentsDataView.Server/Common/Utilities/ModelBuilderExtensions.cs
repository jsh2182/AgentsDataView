using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace AgentsDataView.Common.Utilities
{
    public static class ModelBuilderExtensions
    {
        public static void RegisterAllEntities<BaseType>(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            IEnumerable<Type> types = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && typeof(BaseType).IsAssignableFrom(c.BaseType));
            foreach (Type type in types)
            {
                {
                    modelBuilder.Entity(type);
                }
            }
        }
        public static void AddRestrictDeleteBehaviorConvention(this ModelBuilder modelBuilder, params string[]? exceptionModels)
        {
            IEnumerable<IMutableForeignKey> cascadeFKs = modelBuilder.Model.GetEntityTypes().Where(m=> exceptionModels == null || !exceptionModels.Contains(m.Name))
                .SelectMany(t=>t.GetForeignKeys()).Where(fk=>!fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
            foreach(IMutableForeignKey fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

    }
}
