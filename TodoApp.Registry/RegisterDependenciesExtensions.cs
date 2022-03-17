using Microsoft.Extensions.DependencyInjection;
using TodoApp.DataAccess;
using TodoApp.ItemServices;
using TodoApp.TodoItems.Shared;

namespace TodoApp.Registry
{
    public static class RegisterDependenciesExtensions
    {
        public static IServiceCollection AddAppDependencies(this IServiceCollection services)
        {
            services.Scan(scan =>
                scan.FromAssemblyOf<TodoItemService>().AddClasses(classes => classes.AssignableTo<ITodoItemAdapter>())
                    .AsImplementedInterfaces().WithScopedLifetime());
            services.Scan(scan =>
                scan.FromAssemblyOf<TodoItemRepository>().AddClasses(classes => classes.AssignableTo<ITodoItemsAccess>())
                    .AsImplementedInterfaces().WithScopedLifetime());
            return services.AddDataAccessContext();
        }
    }
}