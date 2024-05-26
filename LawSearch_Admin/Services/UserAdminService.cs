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
            ResponseMessage rm = new();


            var rs_text = await rs.Content.ReadAsStringAsync();
            ResponseMessageLogin? response = JsonSerializer.Deserialize<ResponseMessageLogin>(rs_text.ToString());

            if (rs.IsSuccessStatusCode)
            {
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

                    rm.StatusAPI = true;
                    rm.Message = "Login success";
                }
                else
                {
                    rm.Message = "Login failed";
                    rm.Error = "Invalid response data.";
                }
            }
            else
            {
                if (response?.Exception != null)
                {
                    rm.Message = "Login failed";
                    rm.Error = response.Exception;
                }
                else
                {
                    rm.Message = "Login failed";
                    rm.Error = "An unknown error occurred.";
                }
            }
            return rm;
        }

        public async Task<ResponseMessageListData<User>> GetListUser()
        {
            ResponseMessageListData<User>? rs = await httpClient.GetFromJsonAsync<ResponseMessageListData<User>>($"api/User/GetAll");
            if(rs == null)
            {
                return new ResponseMessageListData<User>
                {
                    Message = "An unknown error occurred.",
                    Exception = "Data is null"
                };
            } else
            {
                if(rs.Status == 200)
                {
                    rs.StatusAPI = true;
                }
            }
            return rs;
        }

        public async Task<ResponseMessageObjectData<User>> UserModifyRole(string userid, string role)
        {
            var body = new
            {
                userid = userid,
                role = role
            };

            HttpResponseMessage rs = await httpClient.PostAsJsonAsync($"api/User/ModifyRole", body);

            var rs_text = await rs.Content.ReadAsStringAsync();
            ResponseMessageObjectData<User>? response = JsonSerializer.Deserialize<ResponseMessageObjectData<User>>(rs_text.ToString());


            if(response == null)
            {
                return new ResponseMessageObjectData<User>
                {
                    Message = "An unknown error occurred.",
                    Exception = "Data is null"
                };
            } else
            {
                if(response.Status == 200)
                {
                    response.StatusAPI = true;
                }
            }

            return response;
        }

        public async Task<ResponseMessage> UserChangeStatus(string userid)
        {
            HttpResponseMessage rs = await httpClient.PostAsJsonAsync($"api/User/ChangeStatus?ID={userid}", "");


            var rs_text = await rs.Content.ReadAsStringAsync();
            ResponseMessage? response = JsonSerializer.Deserialize<ResponseMessage>(rs_text.ToString());


            if (response != null)
            {
                if (response?.Status == 200)
                {
                    response.StatusAPI = true;
                }
            } else
            {
                return new ResponseMessage
                {
                    StatusAPI = false,
                    Message = "An unknown error occurred.",
                    Exception = "Data is null"
                };
            }

            return response;
        }

        public async Task<ResponseMessage> AddUser(string username, string password)
        {
            var body = new
            {
                username = username,
                password = password
            };

            HttpResponseMessage rs = await httpClient.PostAsJsonAsync($"api/Auth/AddUser", body);

            var rs_text = await rs.Content.ReadAsStringAsync();
            ResponseMessage? response = JsonSerializer.Deserialize<ResponseMessage>(rs_text.ToString());


            if (response == null)
            {
                return new ResponseMessage
                {
                    Message = "An unknown error occurred.",
                    Exception = "Data is null"
                };
            }
            else
            {
                if (response.Status == 200)
                {
                    response.StatusAPI = true;
                }
            }

            return response;
        }
    }
}
