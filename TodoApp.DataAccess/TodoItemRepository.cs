using TodoApp.ItemServices;
using TodoApp.ItemServices.Model;

namespace TodoApp.DataAccess
{
    public class TodoItemRepository : EfCoreSqlRepository<TodoItem, TodoContext>, ITodoItemsAccess
    {
        public TodoItemRepository(TodoContext context) : base(context)
        {
        }
    }
}