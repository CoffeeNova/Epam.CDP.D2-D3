using Unity;
using Unity.Lifetime;

namespace FileFormatter.Common
{
    public class Bootstrapper : IBootstrapper
    {
        public IUnityContainer Container { get; }

        public Bootstrapper(IUnityContainer container)
        {
            Container = container;

            RegisterTypes();
        }

        private void RegisterTypes()
        {
            Container.RegisterInstance(new GlobalSettings
            {
                LoggingEnabled = true,
                //Change this setting to switch logging realization 
                LoggingType = GlobalSettings.LoggingAspectType.CodeRewriting

            }, new ContainerControlledLifetimeManager());
            Container.RegisterInstance(new LogMaker((IGlobalSettings)Container.Resolve(typeof(GlobalSettings))), new ContainerControlledLifetimeManager());
        }
    }
}
