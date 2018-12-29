using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;

namespace Draughts.App.Infrastructure.Services
{
    internal interface IAccessService
    {
        Task LogIn(string username, SecureString password);

        Task Register(string username, string email, SecureString password, SecureString confirmPassword);
    }

    internal class AccessService : IAccessService
    {
        private readonly HttpClient _client;

        public AccessService()
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiPath"])
            };
        }

        public async Task LogIn(string username, SecureString password)
        {
            var contentValues = new Dictionary<string, string>
            {
                {"grant_type", "password" },
                {"username", username },
                {"password", password.ToUnsecuredString() },
            };
            var content = new FormUrlEncodedContent(contentValues);
            var response = await _client.PostAsync("Token", content);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var error = JsonConvert.DeserializeObject(result, typeof(Error)) as Error;
                throw new Exception(error?.ErrorDescription);
            }
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", result);
        }

        public async Task Register(string username, string email, SecureString password, SecureString confirmPassword)
        {
            var contentValues = new Dictionary<string, string>
            {
                {"email", email },
                {"username", username },
                {"password", password.ToUnsecuredString() },
                {"confirmpassword", confirmPassword.ToUnsecuredString() }
            };
            var content = new FormUrlEncodedContent(contentValues);
            var response = await _client.PostAsync("api/Account/Register", content);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var obj = new { message = "", ModelState = new Dictionary<string, string[]>() };
                var x = JsonConvert.DeserializeAnonymousType(result, obj);
                throw new Exception(string.Join("\n", x.ModelState.Values.SelectMany(y => y)));
            }
        }

        private class Error
        {
            [JsonProperty("error_description")]
            public string ErrorDescription { get; set; }
        }
    }
}