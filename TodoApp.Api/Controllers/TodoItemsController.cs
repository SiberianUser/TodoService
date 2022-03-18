using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApp.TodoItems.Shared;
using TodoApp.TodoItems.Shared.Dto;

namespace TodoApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemAdapter _itemAdapter;

        public TodoItemsController(ITodoItemAdapter itemAdapter)
        {
            _itemAdapter = itemAdapter;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            return Ok(await _itemAdapter.GetAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            return Ok(await _itemAdapter.GetAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDto)
        {
            if (id != todoItemDto.Id)
            {
                return BadRequest();
            }

            await _itemAdapter.UpdateAsync(id, todoItemDto);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDto)
        {
            var result = await _itemAdapter.AddAsync(todoItemDto);
            return CreatedAtAction(nameof(GetTodoItem), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            await _itemAdapter.DeleteAsync(id);
            return NoContent();
        }
    }
}
