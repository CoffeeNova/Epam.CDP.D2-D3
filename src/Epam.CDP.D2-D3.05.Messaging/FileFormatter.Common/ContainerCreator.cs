using Unity;
using Unity.Interception.ContainerIntegration;

namespace FileFormatter.Common
{
    public static class ContainerCreator
    {
        public static IUnityContainer Create()
        {
            var container = new UnityContainer();

            return container.RegisterBootstrapper<Bootstrapper>().AddNewExtension<Interception>();
        }
    }
}
