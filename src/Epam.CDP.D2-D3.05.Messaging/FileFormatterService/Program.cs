using FileFormatter.Common;

namespace FileFormatterService
{
    class Program
    {
        static void Main()
        {
            var container = ContainerCreator.Create();
            container = container.RegisterBootstrapper<Bootstrapper>();
            var serviceConfigurator = (ServiceConfigurator)container.Resolve(typeof(IServiceConfigurator), null);
            serviceConfigurator.ConfigureAndRun();
        }
    }
}
