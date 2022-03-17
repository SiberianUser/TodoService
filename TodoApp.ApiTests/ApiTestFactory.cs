using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TodoApi.Models;

namespace TodoApp.ApiTests
{
    public class ApiTestFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public long FoundTodoItemId;
        public long NotFoundTodoItemId;
        public long TodoItemToUpdateId;
        public TodoItem TodoItemToUpdate;
        public long TodoItemToDeleteId;
        public List<TodoItem> Source;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<TodoContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<TodoContext>(options =>
                {
                    options.UseInMemoryDatabase("TodoAppTestDb");
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<TodoContext>();
                var logger = scopedServices
                    .GetRequiredService<ILogger<ApiTestFactory<TStartup>>>();

                db.Database.EnsureCreated();

                try
                {
                    Seed(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the " +
                                        "Error: {Message}", ex.Message);
                }
            });
        }

        private void Seed(TodoContext db)
        {
            
            var items = Enumerable.Range(1, 100)
                .Select(s=>RandomTodoItem()).ToList();
            
            db.Set<TodoItem>().AddRangeAsync(items);
            db.SaveChanges();

            FoundTodoItemId = items.First().Id;
            NotFoundTodoItemId = items.Count + 100;
            TodoItemToUpdate = items.Skip(2).First();
            TodoItemToUpdateId = TodoItemToUpdate.Id;
            TodoItemToDeleteId = items.SkipLast(5).Last().Id;
            Source = items;
        }


        private static TodoItem RandomTodoItem() => new Faker<TodoItem>()
            .RuleFor(f => f.IsComplete, f => f.Random.Bool())
            .RuleFor(f => f.Secret, f => f.Random.Word())
            .RuleFor(f => f.Name, f => f.Random.Word()).Generate();
    }
}