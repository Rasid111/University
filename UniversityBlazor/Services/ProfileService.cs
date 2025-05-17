using Blazored.LocalStorage;
using System.Net.Http.Json;
using static UniversityBlazor.Pages.Account.Profile;

namespace UniversityBlazor.Services
{
    public class ProfileService(ILocalStorageService localStorageService, IHttpClientFactory httpClientFactory)
    {
        private readonly IHttpClientFactory httpClientFactory = httpClientFactory;
        public async Task<UserProfile?> GetProfileAsync(string userId)
        {
            var jwt = await localStorageService.GetItemAsStringAsync("jwt");
            var httpClient = httpClientFactory.CreateClient("UniversityBack");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
            var profile = await httpClient.GetFromJsonAsync<UserProfile>($"/api/account/get/{userId}");
            return profile;
        }
    }
}
