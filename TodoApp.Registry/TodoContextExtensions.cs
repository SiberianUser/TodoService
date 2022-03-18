using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.DataAccess;

namespace TodoApp.Registry
{
    internal static class TodoContextExtensions
    {
        public static IServiceCollection AddDataAccessContext(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<TodoContext>(options =>
                 options.UseSqlServer(configuration.GetConnectionString("TodoListSql"), o => o.MigrationsAssembly("TodoApp.Api")));
        }
    }
}