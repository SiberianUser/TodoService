using Microsoft.EntityFrameworkCore;
using TodoApp.ItemServices.Model;

namespace TodoApp.DataAccess
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoItem>().HasKey(c=>c.Id);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}