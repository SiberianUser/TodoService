using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.TodoItems.Shared.Dto;

namespace TodoApp.TodoItems.Shared
{
    public interface ITodoItemAdapter
    {
        Task<IEnumerable<TodoItemDTO>> GetAsync();
        Task<TodoItemDTO> GetAsync(long id);
        Task<TodoItemDTO> AddAsync(TodoItemDTO todoItemDto);
        Task UpdateAsync(long id, TodoItemDTO todoItemDto);
        Task DeleteAsync(long id);
    }
}