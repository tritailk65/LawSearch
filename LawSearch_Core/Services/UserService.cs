using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Services
{
    public class UserService : IUserService
    {
        private readonly IDbService db;

        public UserService(IDbService db)
        {
            this.db = db;
        }

        public User GetUserByID(int id)
        {
            try
            {
                db.OpenConnection();
                string sql = "  select [ID],[username], [password], [Type], [Email] from [user] where ID =" + id;
                DataTable rs = db.ExecuteReaderCommand(sql, "");

                User user = new User();

                if (rs.Rows.Count > 0)
                {
                    user = new User
                    {
                        ID = Globals.GetIDinDT(rs, 0, "ID"),
                        UserName = Globals.GetinDT_String(rs, 0, "username"),
                        Password = Globals.GetinDT_String(rs, 0, "password"),
                        Type = Globals.GetIDinDT(rs, 0, "Type"),
                        Email = Globals.GetinDT_String(rs, 0, "email"),
                    };
                }
                return user;
            } catch
            {
                throw;
            } finally
            {
                db.CloseConnection();
            }
        }

        public User CreateNewUser(User user)
        {
            try
            {
                db.OpenConnection();
                string sql = "INSERT INTO [user] (username, password, Type, email) VALUES (@UserName, @Password, @Type, @Email); SELECT SCOPE_IDENTITY();";
                IDbCommand command = db.CreateCommand(sql);
                db.AddParameterWithValue(command, "@UserName", user.UserName);
                db.AddParameterWithValue(command, "@Password", user.Password);
                db.AddParameterWithValue(command, "@Type", user.Type);
                db.AddParameterWithValue(command, "@Email", user.Email);

                int newUserId = db.ExecuteScalarCommand<int>(command);

                return GetUserByID(newUserId);
            }
            catch
            {
                throw;
            }
            finally
            {
                db.CloseConnection();
            }
        }

    }
}
