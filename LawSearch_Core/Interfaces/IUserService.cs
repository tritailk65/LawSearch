using LawSearch_Core.Models;

namespace LawSearch_Core.Interfaces
{
    public interface IUserService
    {
        User GetUserByID(int id);
        User CreateNewUser(User user);
    }
}
