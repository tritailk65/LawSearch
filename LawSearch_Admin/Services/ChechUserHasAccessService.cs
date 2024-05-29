using LawSearch_Admin.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace LawSearch_Admin.Services
{
    public class ChechUserHasAccessService : ICheckUserHasAccessService
    {
        private readonly AuthenticationStateProvider _stateProvider;

        public ChechUserHasAccessService(AuthenticationStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
        }

        public async Task<bool> CheckUserHasAccessAsync()
        {
            var authState = await _stateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            return user.IsInRole("Admin");
        }
    }
}
