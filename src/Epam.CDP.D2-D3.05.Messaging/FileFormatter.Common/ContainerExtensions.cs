using Unity;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;

namespace FileFormatter.Common
{
    public static class ContainerExtensions
    {
        public static IUnityContainer RegisterBootstrapper<T>(this IUnityContainer container) where T : IBootstrapper
        {
            return container.Resolve<T>().Container;
        }

        public static IUnityContainer RegisterWithInterceptor<TFrom, TTo>(this IUnityContainer container) where TTo : TFrom
        {
            return container.RegisterType<TFrom, TTo>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<UniversalInterceptionBehaviour>());
        }
    }
}
