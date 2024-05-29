using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Interfaces
{
    public interface IUserService
    {
        string GetMyName();
        List<User> GetAllUser();
        User GetUserName(string username);
        void AddUser(User user);
        User ModifyUserRole(int ID, string role);
        User GetUserByID(int ID);
        void ChangeUserStatus(User user);
        void UpdateUserLogin(User user);
    }
}
