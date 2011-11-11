using Autofac;
using Wallboard.Tasks;

namespace Wallboard.Autofac
{
    public class TaskModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IRssTasks).Assembly).Where(x => x.Name.EndsWith("Tasks")).AsImplementedInterfaces();
        }
    }
}