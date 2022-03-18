using System.Collections.Generic;
using Bogus;
using TodoApp.ItemServices.Model;

namespace TodoApp.TestsBase
{
    public static class TodoItemsGenerator
    {
        public static TodoItem RandomTodoItem() => TodoItem.Generate();

        public static List<TodoItem> RandomTodoItems(int count = 100) => TodoItem.Generate(count);

        private static readonly Faker<TodoItem> TodoItem = new Faker<TodoItem>()
            .RuleFor(f => f.IsComplete, f => f.Random.Bool())
            .RuleFor(f => f.Secret, f => f.Random.Word())
            .RuleFor(f => f.Name, f => f.Random.Word());
    }
}