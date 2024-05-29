using Microsoft.AspNetCore.SignalR;

namespace LawSearch_API.Utils
{
    public class AuthenticationHub : Hub
    {
/*        public async Task UserLoggedIn(string userName)
        {
            await Clients.All.SendAsync("UserLoggedIn", userName);
        }*/

        public async Task UserLoggedOut()
        {
            await Clients.All.SendAsync("UserLoggedOut");
        }
    }
}
