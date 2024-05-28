namespace LawSearch_Admin.Interfaces
{
    public interface ICheckUserHasAccessService
    {
        Task<bool> CheckUserHasAccessAsync();
    }
}
