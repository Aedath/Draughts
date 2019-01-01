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
        Task<string> LogIn(string username, SecureString password);

        Task Register(string username, string email, SecureString password, SecureString confirmPassword);

        Task<UserInfo> GetUserInfo();

        Task Logout();

        Task AddGameResult(int generation, int result);

        Task<List<GameResultViewModel>> GetGameResults();

        Task<NeuralNetwork> GetLatestNetwork();

        Task<List<int>> GetGenerations();

        Task<NeuralNetwork> GetByGeneration(int generation);
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

        public async Task<string> LogIn(string username, SecureString password)
        {
            var contentValues = new Dictionary<string, string>
            {
                {"grant_type", "password" },
                {"username", username },
                {"password", password.ToUnsecuredString() },
            };

            var result = await PostAsync<Token>("Token", contentValues);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(result.TokenType, result.AccessToken);

            return result.UserName;
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
            await PostAsync("api/Account/Register", contentValues);
        }

        public async Task AddGameResult(int generation, int result)
        {
            var content = new Dictionary<string, string>
            {
                {"generation", generation.ToString()},
                {"score", result.ToString() }
            };
            await PostAsync("api/GameResults", content);
        }

        public async Task<List<GameResultViewModel>> GetGameResults()
        {
            return await GetAsync<List<GameResultViewModel>>("api/GameResults");
        }

        public async Task<NeuralNetwork> GetLatestNetwork()
        {
            return await GetAsync<NeuralNetwork>("api/NeuralNetworks");
        }

        public async Task<List<int>> GetGenerations()
        {
            var generations = await GetAsync<Generations>("/api/NeuralNetworks/Generations");
            return generations.ExistingGenerations;
        }

        public async Task<NeuralNetwork> GetByGeneration(int generation)
        {
            return await GetAsync<NeuralNetwork>($"/api/NeuralNetworks/{generation}");
        }

        public async Task<UserInfo> GetUserInfo()
        {
            var result = await GetAsync<UserInfo>("api/Account/UserInfo");
            return result;
        }

        public async Task Logout()
        {
            await PostAsync("api/Account/Logout", new Dictionary<string, string>());
            _client.DefaultRequestHeaders.Authorization = null;
        }

        private async Task<T> GetAsync<T>(string path)
        {
            var response = await _client.GetAsync(path);

            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                HandleBadStatusCode(result);
            }

            return JsonConvert.DeserializeObject<T>(result);
        }

        private async Task<string> PostAsync(string path, Dictionary<string, string> data)
        {
            var content = new FormUrlEncodedContent(data);
            var response = await _client.PostAsync(path, content);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                HandleBadStatusCode(result);
            }

            return result;
        }

        private async Task<T> PostAsync<T>(string path, Dictionary<string, string> data)
        {
            var content = new FormUrlEncodedContent(data);
            var response = await _client.PostAsync(path, content);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                HandleBadStatusCode(result);
            }

            return JsonConvert.DeserializeObject<T>(result);
        }

        private static void HandleBadStatusCode(string content)
        {
            if (content.Contains("error_description"))
            {
                throw new Exception(DeserializeError(content));
            }
            throw new Exception(DeserializeModelState(content));
        }

        private static string DeserializeError(string content)
        {
            var error = JsonConvert.DeserializeObject(content, typeof(Error)) as Error;
            return error?.ErrorDescription;
        }

        private static string DeserializeModelState(string content)
        {
            var obj = new { message = "", ModelState = new Dictionary<string, string[]>() };
            var x = JsonConvert.DeserializeAnonymousType(content, obj);
            return string.Join("\n", x.ModelState.Values.SelectMany(y => y));
        }

        private class Error
        {
            [JsonProperty("error_description")]
            public string ErrorDescription { get; set; }
        }

        private class Generations
        {
            [JsonProperty("generations")]
            public List<int> ExistingGenerations { get; set; }
        }
    }

    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }
    }

    public class UserInfo
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class GameResultViewModel
    {
        public int Generation { get; set; }
        public int Score { get; set; }
    }

    public class NeuralNetwork
    {
        public int Generation { get; set; }
        public double[] Network { get; set; }
    }
}