using Blazored.LocalStorage;
using System.Net.Http.Json;
using UniversityBlazor.Pages.Test;
using UniversityBlazor.ViewModels;

namespace UniversityBlazor.Services
{
    public class TestService(ILocalStorageService localStorageService, IHttpClientFactory httpClientFactory)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        public async Task<List<Subject>> GetSubjectsByStudentId(int id)
        {
            var jwt = await localStorageService.GetItemAsStringAsync("jwt");
            var httpClient = httpClientFactory.CreateClient("UniversityBack");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
            var subjects = await httpClient.GetFromJsonAsync<List<Subject>>($"/api/subject/getbyuserid/{id}");
            return subjects is not null ? subjects : [];
        }
        public async Task<ViewModels.Test?> GetTestById(int id)
        {
            var jwt = await localStorageService.GetItemAsStringAsync("jwt");
            var httpClient = httpClientFactory.CreateClient("UniversityBack");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
            var test = await httpClient.GetFromJsonAsync<ViewModels.Test>($"/api/test/get/{id}");
            return test;
        }
        public async Task<TestPassResult?> SubmitTest(int id, List<string> answers)
        {
            var jwt = await localStorageService.GetItemAsStringAsync("jwt");
            var httpClient = httpClientFactory.CreateClient("UniversityBack");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
            var test = await httpClient.PostAsJsonAsync($"/api/test/pass/{id}", answers);
            var res = await test.Content.ReadFromJsonAsync<TestPassResult>();
            return res;
        }
    }
}
