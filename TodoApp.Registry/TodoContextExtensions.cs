using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.DataAccess;

namespace TodoApp.Registry
{
    internal static class TodoContextExtensions
    {
        public static IServiceCollection AddDataAccessContext(this IServiceCollection services)
        {
            return services.AddDbContext<TodoContext>(opt =>
                opt.UseInMemoryDatabase("TodoList"));
        }
    }
}