using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApp.ItemServices.Exceptions;
using TodoApp.ItemServices.Mappers;
using TodoApp.ItemServices.Model;
using TodoApp.TodoItems.Shared;
using TodoApp.TodoItems.Shared.Dto;

namespace TodoApp.ItemServices
{
    public class TodoItemService : ITodoItemAdapter
    {
        private readonly ITodoItemsAccess _dataAccess;
        private readonly TodoItemMapper _itemMapper = new TodoItemMapper();

        public TodoItemService(ITodoItemsAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<IEnumerable<TodoItemDTO>> GetAsync()
        {
            var entities = await _dataAccess.GetAsync();

            return entities.Select(_itemMapper.ItemToDto);
        }

        public async Task<TodoItemDTO> GetAsync(long id)
        {
            var entity = await GetItemAndThrowIfNotExistAsync(id);

            return _itemMapper.ItemToDto(entity);
        }

        public async Task<TodoItemDTO> AddAsync(TodoItemDTO todoItemDto)
        {
            var entity = _itemMapper.DtoToItem(todoItemDto);
            await _dataAccess.AddAsync(entity);
            return _itemMapper.ItemToDto(entity);
        }

        public async Task UpdateAsync(long id, TodoItemDTO todoItemDto)
        {
            var existingEntity = await GetItemAndThrowIfNotExistAsync(id);

            _itemMapper.DtoToItem(todoItemDto, existingEntity);
            await _dataAccess.UpdateAsync(existingEntity);
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await GetItemAndThrowIfNotExistAsync(id);

            await _dataAccess.DeleteAsync(entity);
        }

        private async Task<TodoItem> GetItemAndThrowIfNotExistAsync(long id)
        {
            var entity = await _dataAccess.GetAsync(id);
            if (entity == null)
            {
                throw new TodoItemNotFoundException(id);
            }

            return entity;
        }
    }
}