using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface IUserAdminService
    {
        Task<APIResultSingleVM<User>> UserLogin(LoginVM loginVM);
        Task<ResponseMessageListData<User>> GetListUser();
        Task<ResponseMessageObjectData<User>> UserModifyRole(string userid, string role);
        Task<ResponseMessage> UserChangeStatus(string userid);
        Task<ResponseMessage> AddUser(string username, string password);
        Task UserLogout();
        Task<List<HistorySearch>> GetAllHistorySearch();
    }
}
