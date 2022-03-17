using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using TodoApi;

namespace TodoApp.ApiTests
{
    public abstract class ApiTestsBase
    {
        protected ApiTestFactory<Startup> Factory;
        protected HttpClient Client;
        
        protected abstract string ApiRoute { get; }

        [OneTimeSetUp]
        public void Setup()
        {
            Factory = new ApiTestFactory<Startup>();
            Client = Factory.CreateClient();

            Debug.Assert(Factory.FoundTodoItemId > 0);
            Debug.Assert(Factory.NotFoundTodoItemId > 0);
            Debug.Assert(Factory.TodoItemToUpdateId  > 0);
            Debug.Assert(Factory.TodoItemToUpdate != default);
            Debug.Assert(Factory.TodoItemToDeleteId  > 0);
            Debug.Assert(Factory.Source.Count > 0);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Client.Dispose();
            Factory.Dispose();
        }

        protected async Task<IList<TModel>> GetListAsync<TModel>()
        {
            var response = await Client.GetAsync($"{ApiRoute}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TModel>>(content);
        }
        
        protected async Task<TModel> GetByIdAsync<TModel>(long id)
        {
            var response = await Client.GetAsync($"{ApiRoute}/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TModel>(content);
        }

        protected async Task<TModel> PostAsync<TModel>(TModel model)
        {
            var response = await Client.PostAsync(ApiRoute,
                new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                    "application/json"));

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TModel>(content);
        }
        
        protected async Task PutAsync<TModel>(long id, TModel model, HttpStatusCode expectedStatusCode)
        {
            var response = await Client.PutAsync($"{ApiRoute}/{id}",
                new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                    "application/json"));

            response.StatusCode.Should().Be(expectedStatusCode);
        }

        protected async Task DeleteAsync(long id, HttpStatusCode expectedStatusCode)
        {
            var response = await Client.DeleteAsync($"{ApiRoute}/{id}");

            response.StatusCode.Should().Be(expectedStatusCode);
        }
    }
}