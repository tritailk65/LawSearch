using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface IArticalAdminService
    {
        Task<List<Artical>> GetListArtical();
        
    }
}
