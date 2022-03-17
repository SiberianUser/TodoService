using TodoApp.ItemServices.Model;
using TodoApp.TodoItems.Shared.Dto;

namespace TodoApp.ItemServices.Mappers
{
    public class TodoItemMapper
    {
        public TodoItemDTO ItemToDto(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };     
        
        public TodoItem DtoToItem(TodoItemDTO todoItemDto) =>
            new TodoItem
            {
                Name = todoItemDto.Name,
                IsComplete = todoItemDto.IsComplete
            };

        public void DtoToItem(TodoItemDTO todoItemDto, TodoItem todoItem)
        {
            todoItem.Name = todoItemDto.Name;
            todoItem.IsComplete = todoItemDto.IsComplete;
        }
    }
}