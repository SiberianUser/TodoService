using Bogus;
using TodoApp.TodoItems.Shared.Dto;

namespace TodoApp.TestsBase
{
    public static class TodoItemDtosGenerator
    {
        public static TodoItemDTO RandomTodoItemDto() => TodoItem.Generate();

        private static readonly Faker<TodoItemDTO> TodoItem = new Faker<TodoItemDTO>()
            .RuleFor(f => f.IsComplete, f => f.Random.Bool())
            .RuleFor(f => f.Id, f => f.Random.Long())
            .RuleFor(f => f.Name, f => f.Random.Word());
    }
}