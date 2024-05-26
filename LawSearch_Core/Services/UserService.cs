using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Security.Claims;

namespace LawSearch_Core.Services
{
    public class UserService: IUserService
    {
        private readonly IDbService _dbService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IDbService dbService, IHttpContextAccessor httpContextAccessor)
        {
            _dbService = dbService;
            _httpContextAccessor = httpContextAccessor;
        }

        public void AddUserToList(User user)
        {
            try
            {
                _dbService.OpenConnection();
                string sql = $"Insert into [User] (Username, PasswordHash, PasswordSalt, Role, Status) " +
                    $"Values('{user.Username}','{Convert.ToBase64String(user.PasswordHash)}', '{Convert.ToBase64String(user.PasswordSalt)}', '{user.Role}', '1')";
                _dbService.ExecuteNonQueryCommand(sql);          
            } catch
            {
                throw;
            }
            finally
            {
                _dbService.CloseConnection();
            }
        }

        public List<User> GetAllUser()
        {
            try
            {
                _dbService.OpenConnection();
                string sql = "select * from [User]";
                List<User> users = new List<User>();
                var dt = _dbService.ExecuteReaderCommand(sql, "");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        users.Add(new User
                        {
                            ID = Globals.GetIDinDT(dt,i,"ID"),
                            Username = Globals.GetinDT_String(dt,i,"UserName"),
                            PasswordHash = Convert.FromBase64String(Globals.GetinDT_String(dt,i,"PasswordHash")),
                            PasswordSalt = Convert.FromBase64String(Globals.GetinDT_String(dt,i,"PasswordSalt")),
                            Role = Globals.GetinDT_String(dt,i,"Role"),
                            Status = Convert.ToBoolean(Globals.GetIDinDT(dt, i, "Status"))
                        });
                    }
                }
                return users;
            } catch (Exception ex)
            {
                throw new BadRequestException(ex.ToString(), 400, 400);
            } finally
            {
                _dbService.CloseConnection();
            }
        }

        public string GetMyName()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                var nameClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name);
                if (nameClaim != null)
                {
                    result = nameClaim.Value;
                }
            }
            return result;
        }

        public User GetUserName(string username)
        {
            try
            {
                _dbService.OpenConnection();

                string sql = $"select top(1) * from [User] where Username = '{username}'";
                var dt = _dbService.ExecuteReaderCommand(sql, "");
                User user = new User();
                if( dt.Rows.Count > 0)
                {
                    user = new User
                    {
                        ID = Globals.GetIDinDT(dt, 0, "ID"),
                        Username = Globals.GetinDT_String(dt, 0, "Username"),
                        PasswordHash = Convert.FromBase64String(Globals.GetinDT_String(dt, 0, "PasswordHash")),
                        PasswordSalt = Convert.FromBase64String(Globals.GetinDT_String(dt, 0, "PasswordSalt")),
                        Role = Globals.GetinDT_String(dt, 0, "Role"),
                        Status = Convert.ToBoolean(Globals.GetIDinDT(dt,0,"Status"))
                    };
                    return user;
                }
                return null;
            }
            catch
            {
                throw;
            }
            finally { _dbService.CloseConnection(); }
        }

        public User GetUserByID(int ID)
        {
            try
            {
                _dbService.OpenConnection();

                string sql = $"select top(1) * from [User] where ID = '{ID}'";
                var dt = _dbService.ExecuteReaderCommand(sql, "");
                User user = new User();
                if (dt.Rows.Count > 0)
                {
                    user = new User
                    {
                        ID = Globals.GetIDinDT(dt, 0, "ID"),
                        Username = Globals.GetinDT_String(dt, 0, "Username"),
                        PasswordHash = Convert.FromBase64String(Globals.GetinDT_String(dt, 0, "PasswordHash")),
                        PasswordSalt = Convert.FromBase64String(Globals.GetinDT_String(dt, 0, "PasswordSalt")),
                        Role = Globals.GetinDT_String(dt, 0, "Role"),
                        Status = Convert.ToBoolean(Globals.GetIDinDT(dt, 0, "Status"))
                    };
                    return user;
                }
                return null;
            }
            catch
            {
                throw;
            }
            finally { _dbService.CloseConnection(); }
        }

        public User ModifyUserRole(int ID, string role)
        {
            try
            {
                _dbService.OpenConnection();

                string sqlUpdate = $"Update [User] " +
                    $"Set Role = '{role}' " +
                    $"Where ID = {ID}";
                _dbService.ExecuteNonQueryCommand(sqlUpdate);

                string sqlGet = $"Select * from [User] where ID = {ID}";
                var dt = _dbService.ExecuteReaderCommand(sqlGet, "");
                User user = new User();
                user = new User
                {
                    ID = Globals.GetIDinDT(dt, 0, "ID"),
                    Username = Globals.GetinDT_String(dt, 0, "Username"),
                    Role = Globals.GetinDT_String(dt, 0, "Role"),
                    Status = Convert.ToBoolean(Globals.GetIDinDT(dt, 0, "Status"))
                };
                return user;
            } catch
            {
                throw;
            } finally { _dbService.CloseConnection(); }
        }

        public void ChangeUserStatus(User user)
        {
            try
            {
                _dbService.OpenConnection();
                string sql = $"Update [User] " +
                    $"Set Status = {Convert.ToInt32(!user.Status)} " +
                    $"Where ID = {user.ID}";
                _dbService.ExecuteNonQueryCommand(sql);

            } catch
            {
                throw;
            } finally
            {
                _dbService.CloseConnection();
            }
        }
    }
}
