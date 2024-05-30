using LawSearch_Admin.Extensions;
using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LawSearch_Admin.Services
{
    public class UserAdminService : IUserAdminService
    {
        private HttpClient httpClient;
        private readonly ICookieService cookie;
        private readonly AuthenticationStateProvider _stateProvider;

        public UserAdminService(HttpClient httpClient, ICookieService cookie, AuthenticationStateProvider stateProvider, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.cookie = cookie;
            _stateProvider = stateProvider;
        }

        public async Task<APIResultSingleVM<User>> UserLogin(LoginVM loginVM)
        { 
            try
            {
                APIResultSingleVM<User> apiResponse = new APIResultSingleVM<User>();
                var rs = await httpClient.PostAsJsonAsync($"api/Auth/Login",loginVM);
                var rs_text = await rs.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                apiResponse = JsonSerializer.Deserialize<APIResultSingleVM<User>>(rs_text, options);

                if (apiResponse != null && apiResponse.Status == 200)
                {                   
                    // Save data to cookie
                    if (apiResponse.Data.ID != null)
                    {
                        await cookie.SetValue(CookieKeys.userid, $"{apiResponse.Data.ID}");
                    }
                    if (apiResponse.Data.Username != null)
                    {
                        await cookie.SetValue(CookieKeys.username, apiResponse.Data.Username);
                    }
                    if (apiResponse.Data.Role != null)
                    {
                        await cookie.SetValue(CookieKeys.userrole, apiResponse.Data.Role);
                    }
                    if (apiResponse.Data.Token != null)
                    {
                        await cookie.SetValue(CookieKeys.authToken, apiResponse.Data.Token);
                    }

                    ((ApiAuthenticationStateProvider)_stateProvider).MarkUserAsAuthenticated(loginVM.Username);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiResponse.Data.Token);
                }               

                return apiResponse;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
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

        public async Task UserLogout()
        {
            await cookie.DeleteAllValue();
            ((ApiAuthenticationStateProvider)_stateProvider).MarkUserAsLoggedOut();
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
