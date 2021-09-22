using Client.Interfaces;
using Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using SharedObjects;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly ILocationStorageService _localStorageService;
        private readonly IConfiguration _configuration;

        public HttpService(HttpClient httpClient, NavigationManager navigationManager, ILocationStorageService localStorageService, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _configuration = configuration;
        }
        public async Task<T> Delete<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            return await sendRequest<T>(request);
        }

        public async Task<T> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await sendRequest<T>(request);
        }

        public async Task<T> Post<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return await sendRequest<T>(request);
        }

        public async Task<T> Put<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return await sendRequest<T>(request);

        }

        private async Task<T> sendRequest<T>(HttpRequestMessage request)
        {
            var user = await _localStorageService.GetItem<User>("user");
            var isApi = !request.RequestUri.IsAbsoluteUri;
            if (user != null && isApi)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
            using var response =  await _httpClient.SendAsync(request);
            if(response.StatusCode == HttpStatusCode.Unauthorized)
            {
                //var bytes = await response.Content.ReadAsStreamAsync();
                //StreamReader streamReader = new StreamReader(bytes);
                //string text = streamReader.ReadToEnd();
                //dynamic myProperty = JsonSerializer.Deserialize<ExpandoObject>(text);
                var content = response.Content.ReadAsStringAsync();
                _navigationManager.NavigateTo("/");
                return default;
            }
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<string>();
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                throw new Exception(error["message"]);
            }
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }

    public class Errors
    {
        public List<string> Message { get; set; }
    }
}

