using TodoApp.TodoItems.Shared.Exceptions;

namespace TodoApp.ItemServices.Exceptions
{
    public class TodoItemNotFoundException : NotFoundException
    {
        public TodoItemNotFoundException(long todoItemId) : base($"Todo item with id {todoItemId} not found")
        {
        }
    }
}