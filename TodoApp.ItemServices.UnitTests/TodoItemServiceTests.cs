using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using TodoApp.ItemServices.Exceptions;
using TodoApp.ItemServices.Model;
using TodoApp.TestsBase;
using TodoApp.TodoItems.Shared;
using TodoApp.TodoItems.Shared.Dto;

namespace TodoApp.ItemServices.UnitTests
{
    public class TodoItemServiceTests
    {
        private readonly AutoMocker _mocker = new AutoMocker(MockBehavior.Strict);
        private ITodoItemAdapter _subject;

        [SetUp]
        public void BeforeEachTest()
        {
            _subject = _mocker.CreateInstance<TodoItemService>();
        }

        [Test]
        public async Task GetAsync_VerifyResult()
        {
            var items = TodoItemsGenerator.RandomTodoItems();
            _mocker.GetMock<ITodoItemsAccess>().Setup(x => x.GetAsync()).ReturnsAsync(items);

            var result = await _subject.GetAsync();

            result.ToList().Should().BeEquivalentTo(items, opts => opts.Excluding(e => e.Secret));
        }

        [Test]
        public async Task GetAsync_NoItems_VerifyEmptyResult()
        {
            _mocker.GetMock<ITodoItemsAccess>().Setup(x => x.GetAsync()).ReturnsAsync(new TodoItem[] { });

            var result = await _subject.GetAsync();

            result.ToList().Should().BeEmpty();
        }

        [Test]
        public async Task GetAsync_ById_VerifyResult()
        {
            var item = TodoItemsGenerator.RandomTodoItem();
            _mocker.GetMock<ITodoItemsAccess>().Setup(x => x.GetAsync(It.IsAny<long>())).ReturnsAsync(item);

            var result = await _subject.GetAsync(It.IsAny<long>());

            result.Should().BeEquivalentTo(item, opts => opts.Excluding(x => x.Secret));
        }

        [Test]
        public async Task GetAsync_ById_VerifyArguments()
        {
            var idCallback = 0L;
            _mocker.GetMock<ITodoItemsAccess>().Setup(x => x.GetAsync(It.IsAny<long>()))
                .Callback<long>(c => idCallback = c)
                .ReturnsAsync(TodoItemsGenerator.RandomTodoItem());

            await _subject.GetAsync(123L);

            _mocker.GetMock<ITodoItemsAccess>().Verify(x => x.GetAsync(It.IsAny<long>()), Times.Once);
            idCallback.Should().Be(123L);
        }

        [Test]
        public async Task AddAsync_VerifyAddedItemCallback()
        {
            var itemDto = TodoItemDtosGenerator.RandomTodoItemDto();
            TodoItem addCallback = null;
            _mocker.GetMock<ITodoItemsAccess>().Setup(x => x.AddAsync(It.IsAny<TodoItem>()))
                .Callback<TodoItem>(c => addCallback = c)
                .Returns(Task.CompletedTask);

            await _subject.AddAsync(itemDto);

            addCallback.Should().NotBeNull();
            addCallback.Should().BeEquivalentTo(itemDto, opts => opts.Excluding(x => x.Id));
        }

        [Test]
        public async Task AddAsync_VerifyAddedItemMappedResult()
        {
            var itemDto = TodoItemDtosGenerator.RandomTodoItemDto();
            _mocker.GetMock<ITodoItemsAccess>().Setup(x => x.AddAsync(It.IsAny<TodoItem>()))
                .Returns(Task.CompletedTask)
                .Callback<TodoItem>(c =>
                {
                    c.Id = 333L;
                });

            var result = await _subject.AddAsync(itemDto);
            result.Should().BeEquivalentTo(itemDto, opts => opts.Excluding(x => x.Id));
            result.Id.Should().Be(333L);
        }

        [Test]
        public async Task UpdateAsync_VerifyUpdatedItemCallback()
        {
            var itemDto = TodoItemDtosGenerator.RandomTodoItemDto();
            var item = TodoItemsGenerator.RandomTodoItem();
            TodoItem updateCallback = null;
            _mocker.GetMock<ITodoItemsAccess>().Setup(x => x.GetAsync(itemDto.Id))
                .ReturnsAsync(item);
            _mocker.GetMock<ITodoItemsAccess>().Setup(x => x.UpdateAsync(It.IsAny<TodoItem>()))
                .Callback<TodoItem>(c => updateCallback = c)
                .Returns(Task.CompletedTask);

            await _subject.UpdateAsync(itemDto.Id, itemDto);

            updateCallback.Should().NotBeNull();
            updateCallback.Should().BeEquivalentTo(itemDto, opts => opts.Excluding(x => x.Id));
            updateCallback.Id.Should().Be(item.Id);
        }

        [Test]
        public async Task UpdateAsync_ItemNotExist_VerifyNotFoundException()
        {
            _mocker.GetMock<ITodoItemsAccess>().Setup(x => x.GetAsync(It.IsAny<long>()))
                .ReturnsAsync((TodoItem)null);

            Func<Task> act = async () => await _subject.UpdateAsync(It.IsAny<long>(), It.IsAny<TodoItemDTO>());

            await act.Should().ThrowAsync<TodoItemNotFoundException>();
        }

        [Test]
        public async Task DeleteAsync_VerifyDeletedCallback()
        {
            var itemToDeleteId = 123L;
            var item = TodoItemsGenerator.RandomTodoItem();
            TodoItem deleteCallback = null;
            _mocker.GetMock<ITodoItemsAccess>().Setup(x => x.GetAsync(itemToDeleteId))
                .ReturnsAsync(item);
            _mocker.GetMock<ITodoItemsAccess>().Setup(x => x.DeleteAsync(It.IsAny<TodoItem>()))
                .Callback<TodoItem>(c => deleteCallback = c)
                .Returns(Task.CompletedTask);

            await _subject.DeleteAsync(itemToDeleteId);

            deleteCallback.Should().NotBeNull();
            deleteCallback.Should().Be(item);
        }

        [Test]
        public async Task DeleteAsync_ItemNotExist_VerifyNotFoundException()
        {
            _mocker.GetMock<ITodoItemsAccess>().Setup(x => x.GetAsync(It.IsAny<long>()))
                .ReturnsAsync((TodoItem)null);

            Func<Task> act = async () => await _subject.DeleteAsync(It.IsAny<long>());

            await act.Should().ThrowAsync<TodoItemNotFoundException>();
        }

        [TearDown]
        public void AfterEachTest()
        {
            _mocker.GetMock<ITodoItemsAccess>().Invocations.Clear();
        }
    }
}