using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface IUserAdminService
    {
        Task<ResponseMessage> UserLogin(string username, string password);
    }
}
