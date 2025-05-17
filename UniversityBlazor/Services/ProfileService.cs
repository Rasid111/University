using Blazored.LocalStorage;
using System.Net.Http.Json;
using UniversityBlazor.ViewModels;
using static UniversityBlazor.Pages.Account.Profile;

namespace UniversityBlazor.Services
{
    public class ProfileService(ILocalStorageService localStorageService, IHttpClientFactory httpClientFactory)
    {
        private readonly IHttpClientFactory httpClientFactory = httpClientFactory;
        public async Task<User?> GetProfileAsync(string userId)
        {
            var jwt = await localStorageService.GetItemAsStringAsync("jwt");
            var httpClient = httpClientFactory.CreateClient("UniversityBack");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
            var profile = await httpClient.GetFromJsonAsync<User>($"/api/account/get/{userId}");
            return profile;
        }
        public async Task<List<TeacherGroupSubject>?> GetGroupScheduleAsync(int groupId)
        {
            var jwt = await localStorageService.GetItemAsStringAsync("jwt");
            var httpClient = httpClientFactory.CreateClient("UniversityBack");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
            var tgss = await httpClient.GetFromJsonAsync<List<TeacherGroupSubject>>($"/api/teachergroupsubject/getschedulebygroupid/{groupId}");
            return tgss;
        }
        public async Task<List<TeacherGroupSubject>?> GetTeacherScheduleAsync(int teacherId)
        {
            var jwt = await localStorageService.GetItemAsStringAsync("jwt");
            var httpClient = httpClientFactory.CreateClient("UniversityBack");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
            var tgss = await httpClient.GetFromJsonAsync<List<TeacherGroupSubject>>($"/api/teachergroupsubject/getschedulebyteacherid/{teacherId}");
            return tgss;
        }
        
    }
}
