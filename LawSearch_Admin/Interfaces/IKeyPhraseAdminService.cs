using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface IKeyPhraseAdminService
    {
        Task<List<KeyPhrase>> GetListKeyPhrase();

        Task<List<KeyPhraseRelate>> GetKeyPhraseRelates(int id);
    }
}
