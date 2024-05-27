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
            
            try
            {
                ResponseMessageLogin? response = JsonSerializer.Deserialize<ResponseMessageLogin>(rs_text.ToString());

                if (response != null)
                {
                    if (response.Status == 200)
                    {
                        rm.StatusAPI = true;

                        try
                        {
                            // Save data to cookie
                            if(response?.Data?.Id != null)
                            {
                                await cookie.SetValue(CookieKeys.userid, $"{response.Data.Id}");
                            }
                            if(response?.Data?.Username != null)
                            {
                                await cookie.SetValue(CookieKeys.username, response.Data.Username);
                            }
                            if(response?.Data?.Role != null)
                            {
                                await cookie.SetValue(CookieKeys.userrole, response.Data.Role);
                            }
                            if(response?.Data?.Token != null)
                            {
                                await cookie.SetValue(CookieKeys.authToken, response.Data.Token);
                            }
                            await cookie.SetValue(CookieKeys.password, password);

                            // Authorization api with token
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Data.Token);
                        }
                        catch (Exception ex)
                        {
                            rm.Message = ex.Message;
                        }
                    }
                    else
                    {
                        rm.Message = response.Message;
                    }
                } else
                {
                    rm.Message = "An error occurred when data is empty!";
                }
            } catch (Exception ex)
            {
                rm.Message = ex.Message;
            }

            return rm;
        }

        public async Task<ResponseMessageListData<User>> GetListUser()
        {
            ResponseMessageListData<User>? response = new ResponseMessageListData<User>();

            try
            {
                var rs = await httpClient.GetFromJsonAsync<ResponseMessageListData<User>>($"api/User/GetAll");

                if (rs != null)
                {
                    response = rs;

                    if (response.Status == 200)
                    {
                        response.StatusAPI = true;
                    }
                } else
                {
                    response.Message = "An error occurred when data is empty!";
                }
            } catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
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
                    Message = "An error occurred when data is empty!"
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
            ResponseMessage response = new ResponseMessage();

            HttpResponseMessage rs = await httpClient.PostAsJsonAsync($"api/User/ChangeStatus?ID={userid}", "");

            string rs_text = await rs.Content.ReadAsStringAsync();

            try
            {
                var temp = JsonSerializer.Deserialize<ResponseMessage>(rs_text);

                if(temp == null)
                {
                    response.Message = "An error occurred when data is empty!";
                } else
                {
                    response = temp;

                    if (response?.Status == 200)
                    {
                        response.StatusAPI = true;
                    }
                }
            } catch (Exception ex)
            {
                response.Message = ex.Message;
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
                    Message = "An unknown error occurred [Data is null]"
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
