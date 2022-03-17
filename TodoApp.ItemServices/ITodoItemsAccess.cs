using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.ItemServices.Model;

namespace TodoApp.ItemServices
{
    public interface ITodoItemsAccess
    {
        Task<IEnumerable<TodoItem>> GetAsync();
        Task<TodoItem> GetAsync(long id);
        Task AddAsync(TodoItem todoItem);
        Task UpdateAsync(TodoItem todoItem);
        Task DeleteAsync(TodoItem todoItem);
    }
}