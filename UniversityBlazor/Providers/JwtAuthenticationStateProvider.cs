using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using UniversityBlazor.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace UniversityBlazor.Providers;

public class JwtAuthenticationStateProvider(
    ILocalStorageService localStorageService,
    IHttpClientFactory httpClientFactory) : AuthenticationStateProvider
{
    private readonly ILocalStorageService localStorageService = localStorageService;
    private readonly IHttpClientFactory httpClientFactory = httpClientFactory;
    private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler = new();

    private async Task<ClaimsIdentity?> GetClaimsIdentity()
    {
        var jwt = await localStorageService.GetItemAsStringAsync("jwt");
        var refresh = await localStorageService.GetItemAsStringAsync("refresh");
        if (string.IsNullOrWhiteSpace(jwt) || string.IsNullOrWhiteSpace(refresh))
            return null;

        var result = await jwtSecurityTokenHandler.ValidateTokenAsync(
            jwt,
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "MyApplication",

                ValidateAudience = true,
                ValidAudience = "Big Company",

                SignatureValidator = (token, validationParameters) => new JwtSecurityToken(token),

                RequireExpirationTime = true,
                ValidateLifetime = true,

                LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires > DateTime.UtcNow,
            });

        if (result.IsValid == false)
        {
            if (result.Exception is SecurityTokenInvalidLifetimeException)
            {
                var identityServiceHttpClient = httpClientFactory.CreateClient("IdentityService");
                identityServiceHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
                var httpResult = await identityServiceHttpClient.PutAsync($"/api/Identity/Token?refresh={refresh}", null);

                if (httpResult.IsSuccessStatusCode == false)
                {
                    return null;
                }

                var refreshAccessTokens = await httpResult.Content.ReadFromJsonAsync<RefreshAccessTokens>();

                if (refreshAccessTokens is null)
                {
                    return null;
                }

                await localStorageService.SetItemAsStringAsync("jwt", refreshAccessTokens.Access);
                await localStorageService.SetItemAsStringAsync("refresh", refreshAccessTokens.Refresh);

                var newTokenObj = jwtSecurityTokenHandler.ReadJwtToken(jwt);
                return new ClaimsIdentity(newTokenObj.Claims, "jwt");
            }

            return null;
        }

        var tokenObj = jwtSecurityTokenHandler.ReadJwtToken(jwt);

        return new ClaimsIdentity(tokenObj.Claims, "jwt");
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsIdentity = await GetClaimsIdentity();

        var claimsPrincipal = claimsIdentity == null
            ? new ClaimsPrincipal()
            : new ClaimsPrincipal(claimsIdentity);

        var authenticationState = new AuthenticationState(claimsPrincipal);

        return authenticationState;
    }
}