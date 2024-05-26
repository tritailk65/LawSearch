using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
namespace LawSearch_Admin.Services
{
    public class UserAdminService : IUserAdminService
    {
        private HttpClient httpClient;
        private readonly ICookie cookie;

        public UserAdminService(HttpClient _httpClient, ICookie _cookie)
        {
            httpClient = _httpClient;
            cookie = _cookie;
        }

        public async Task<ResponseMessage> UserLogin(string username, string password)
        {
            var body = new
            {
                username = username,
                password = password
            };

            HttpResponseMessage rs = await httpClient.PostAsJsonAsync($"api/Auth/Login", body);
            var a = await rs.Content.ReadAsStringAsync();

            ResponseMessage rm = new();
            if (rs.IsSuccessStatusCode)
            {
                var rs_text = await rs.Content.ReadAsStringAsync();
                ResponseMessageLogin? response = JsonSerializer.Deserialize<ResponseMessageLogin>(rs_text.ToString());
                if (response?.Data != null)
                {
                    try
                    {
                        await cookie.SetValue(CookieKeys.userid, response.Data.Id.ToString());
                        await cookie.SetValue(CookieKeys.username, response.Data.Username);
                        await cookie.SetValue(CookieKeys.userrole, response.Data.Role);
                        await cookie.SetValue(CookieKeys.authToken, response.Data.Token);
                        await cookie.SetValue(CookieKeys.password, password);

                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Data.Token);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    rm.Status = true;
                    rm.Message = "Login success";
                }
                else
                {
                    rm.Status = false;
                    rm.Message = "Login failed";
                    rm.Error = "Invalid response data.";
                }
            }
            else
            {
                // Read the error message from the response
                var resultPost = await rs.Content.ReadFromJsonAsync<APIResultVM>();
                if (resultPost != null && resultPost.Message != null)
                {
                    rm.Message = "Login failed";
                    rm.Error = resultPost.Message;
                }
                else
                {
                    rm.Message = "Login failed";
                    rm.Error = "An unknown error occurred.";
                }
            }
            return rm;
        }
    }
}
