using Client.Interfaces;
using Domain;
using Microsoft.AspNetCore.Components;
using SharedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpService _httpService;
        private readonly NavigationManager _navigationManager;
        private readonly ILocationStorageService _locationStorageService;

        public AuthenticationService(IHttpService httpService, NavigationManager navigationManager, ILocationStorageService locationStorageService)
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _locationStorageService = locationStorageService;
        }
        public User User { get; private set; }

        public async Task Initialize()
        {
            User = await _locationStorageService.GetItem<User>("user");
        }

        public async Task Login(string email, string password)
        {
            User = await _httpService.Post<User>("api/user/login", new { email, password });
            if(User == null)
            {
                throw new Exception("Invalid login attempt");
            }
            else
            {
                await _locationStorageService.SetItem("user", User);
            }
        }


        public async Task Logout()
        {
            User = null;
            await _locationStorageService.RemoveItem("user");
            _navigationManager.NavigateTo("/");
        }
    }
}
