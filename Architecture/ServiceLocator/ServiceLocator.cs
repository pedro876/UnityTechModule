using System;
using System.Collections.Generic;
using System.Linq;

namespace Architecture
{
    public static partial class ServiceLocator
    {
        #region Variables

        private static readonly Dictionary<Type, IService> services = new();
        public static bool IsEnable { get; private set; }

        #endregion

        #region Base Methods

        private static void Initialize()
        {
            var interfaces = typeof(IService).Assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IService))).ToArray();

            foreach (var service in interfaces) services[service] = Activator.CreateInstance(service) as IService;
            foreach (var service in services.Values) service.Initialize();

            IsEnable = true;
        }

        public static T GetService<T>() where T : IService
        {
            if (services.ContainsKey(typeof(T))) return (T)services[typeof(T)];

            throw new Exception($"The service: {typeof(T).Name} is not register");
        }

        private static void Dispose()
        {
            IsEnable = false;

            foreach (var service in services.Values)
            {
                service.Dispose();
            }

            services.Clear();
        }

        #endregion
    }
}
