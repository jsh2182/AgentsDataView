using System.Reflection;

namespace AgentsDataView.Services
{
    public static class ServiceHelper
    {
        public static void AddScopedServices(this IServiceCollection services, Type baseService, Type baseImplementation)
        {
            Assembly assemby = baseImplementation.Assembly;
            var types = assemby.GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic &&
               t.BaseType != null &&
                _inheritsFrom(t.BaseType, baseImplementation)).ToList();
            foreach (var type in types)
            {

                //var iface = type.GetInterfaces().FirstOrDefault(i => _inheritsFrom(i, baseService) && i.IsGenericType);
                var iface = _findInterface(type, baseService);
                if (iface != null)
                    services.AddScoped(iface, type);
            }
        }
        private static bool _inheritsFrom(Type type, Type baseType)
        {
            // ✅ اگر پایه غیرجنریک است:
            if (!baseType.IsGenericTypeDefinition)
            {
                return baseType.IsAssignableFrom(type) && type != baseType;
            }

            // ✅ اگر پایه جنریک است:
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                    return true;

                type = type.BaseType;
            }

            return false;
        }
        private static Type? _findInterface(Type type, Type baseService)
        {
            string baseName = baseService.IsGenericTypeDefinition
                                            ? baseService.Name.Substring(0, baseService.Name.IndexOf('`'))
                                                : baseService.Name;
            foreach (var iface in type.GetInterfaces())
            {
                string ifaceName = iface.IsGenericType
                    ? iface.GetGenericTypeDefinition().Name.Substring(0, iface.GetGenericTypeDefinition().Name.IndexOf('`'))
                    : iface.Name;

                if (ifaceName != baseName && iface != type)
                    return iface;
            }

            return null;
        }
    }
}
