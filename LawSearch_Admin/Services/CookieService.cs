using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using Microsoft.JSInterop;

namespace LawSearch_Admin.Services
{
    public class CookieService : ICookieService
    {
        readonly IJSRuntime JSRuntime;
        string expires = "";

        public CookieService(IJSRuntime jsRuntime)
        {
            this.JSRuntime = jsRuntime;
            ExpireDays = 300;
        }

        public async Task SetValue(string key, string value, int? days = null)
        {
            try
            {
                var curExp = days != null ? days > 0 ? DateToUTC(days.Value) : "" : expires;
                await SetCookie($"{key}={value}; expires={curExp}; path=/");
            } catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task<string> GetValue(string key, string def = "")
        {
            var cValue = await GetCookie();
            if (string.IsNullOrEmpty(cValue)) return def;

            var vals = cValue.Split(';');
            foreach (var val in vals)
                if (!string.IsNullOrEmpty(val) && val.IndexOf('=') > 0)
                    if (val.Substring(0, val.IndexOf('=')).Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                        return val.Substring(val.IndexOf('=') + 1);
            return def;
        }

        private async Task SetCookie(string value)
        {
            await JSRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{value}\"");
        }

        private async Task<string> GetCookie()
        {
            return await JSRuntime.InvokeAsync<string>("eval", $"document.cookie");
        }

        public int ExpireDays
        {
            set => expires = DateToUTC(value);
        }

        private static string DateToUTC(int days) => DateTime.Now.AddDays(days).ToUniversalTime().ToString("R");

        public async Task DeleteAllValue()
        {
            await SetCookie($"{CookieKeys.authToken}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/");
            await SetCookie($"{CookieKeys.userid}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/");
            await SetCookie($"{CookieKeys.username}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/");
            await SetCookie($"{CookieKeys.password}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/");
            await SetCookie($"{CookieKeys.userrole}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/");
        }
    }
}
