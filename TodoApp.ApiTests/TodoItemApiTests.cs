using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TodoApi.Models;

namespace TodoApp.ApiTests
{
    public class TodoItemApiTests:ApiTestsBase
    {
        protected override string ApiRoute => "api/todoitems";

        [Test, Order(0)]
        public async Task Get_VerifyResult()
        {
            var response = await GetListAsync<TodoItemDTO>();

            response.Count.Should().Be(Factory.Source.Count);
        }

        [Test]
        public async Task GetById_VerifyResult()
        {
            var todoItem = Factory.Source.Find(f => f.Id == Factory.FoundTodoItemId);
            var response = await GetByIdAsync<TodoItemDTO>(Factory.FoundTodoItemId);

            response.Should().BeEquivalentTo(todoItem, opts=>opts.Excluding(x=>x.Secret));
        }
        
        [Test]
        public async Task GetById_VerifyNotFound()
        {
            var response = await Client.GetAsync($"{ApiRoute}/{Factory.NotFoundTodoItemId}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Post_VerifySuccess()
        {
            var todoItem = new TodoItemDTO
            {
                IsComplete = true,
                Name = "NewTestTodoItem"
            };
            var getAll = await GetListAsync<TodoItemDTO>();
            var oldCount = getAll.Count;

            var result = await PostAsync(todoItem);

            result.Should().BeEquivalentTo(todoItem, opts=> opts.Excluding(x=>x.Id));
            getAll = await GetListAsync<TodoItemDTO>();
            getAll.Should().Contain(x => x.Id == result.Id);
            getAll.Count.Should().Be(oldCount + 1);
            var getResult = await GetByIdAsync<TodoItemDTO>(result.Id);
            getResult.Should().BeEquivalentTo(result);
        }
        
        [Test]
        public async Task Put_VerifySuccess()
        {
            var todoItem = new TodoItemDTO
            {
                IsComplete = true,
                Name = "NewTestTodoItem",
                Id = Factory.TodoItemToUpdateId
            };
            var getAll = await GetListAsync<TodoItemDTO>();
            var oldCount = getAll.Count;

            await PutAsync(Factory.TodoItemToUpdateId, todoItem, HttpStatusCode.NoContent);

            getAll = await GetListAsync<TodoItemDTO>();
            getAll.Should().Contain(x => x.Id == Factory.TodoItemToUpdateId);
            getAll.Count.Should().Be(oldCount);
            var getResult = await GetByIdAsync<TodoItemDTO>(Factory.TodoItemToUpdateId);
            getResult.Should().BeEquivalentTo(todoItem);
        }
        
        [Test]
        public async Task Put_IdNotMatch_VerifyBadRequest()
        {
            var todoItem = new TodoItemDTO
            {
                IsComplete = true,
                Name = "NewTestTodoItem",
                Id = Factory.TodoItemToUpdateId
            };
            var getAll = await GetListAsync<TodoItemDTO>();
            var oldCount = getAll.Count;
            var itemToUpdate = getAll.First(x => x.Id == Factory.TodoItemToUpdateId);

            await PutAsync(Factory.TodoItemToDeleteId, todoItem, HttpStatusCode.BadRequest);

            getAll = await GetListAsync<TodoItemDTO>();
            getAll.Should().Contain(x => x.Id == Factory.TodoItemToUpdateId);
            getAll.Count.Should().Be(oldCount);
            var itemShouldNotBeUpdated = getAll.First(x => x.Id == Factory.TodoItemToUpdateId);
            itemToUpdate.Should().BeEquivalentTo(itemShouldNotBeUpdated);
        }
        
        [Test]
        public async Task Put_VerifyNotFound()
        {
            var todoItem = new TodoItemDTO
            {
                IsComplete = true,
                Name = "NewTestTodoItem",
                Id = Factory.NotFoundTodoItemId
            };

            await PutAsync(Factory.NotFoundTodoItemId, todoItem, HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task Delete_VerifyNotFound()
        {
            await DeleteAsync(Factory.NotFoundTodoItemId, HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task Delete_VerifySuccess()
        {
            var before = await GetListAsync<TodoItemDTO>();
            before.Should().Contain(x => x.Id == Factory.TodoItemToDeleteId);

            await DeleteAsync(Factory.TodoItemToDeleteId, HttpStatusCode.NoContent);
            
            var after = await GetListAsync<TodoItemDTO>();
            after.Count.Should().Be(before.Count - 1);
            after.Should().NotContain(x => x.Id == Factory.TodoItemToDeleteId);
        }
    }
}