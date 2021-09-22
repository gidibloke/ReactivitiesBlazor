using Client.Interfaces;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Services
{
    public class LocalStorageService : ILocationStorageService
    {
        private readonly IJSRuntime _jSRuntime;

        public LocalStorageService(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }


        public async Task<T> GetItem<T>(string key)
        {
            var json = await _jSRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (json == null)
                return default;
            return JsonSerializer.Deserialize<T>(json);        }

        public async Task RemoveItem(string key)
        {
            await _jSRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task SetItem<T>(string key, T value)
        {            
            await _jSRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));

        }
    }
}
