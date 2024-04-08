using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface IKeyPhraseAdminService
    {
        Task<List<KeyPhrase>> GetListKeyPhrase();

        Task<List<KeyPhraseRelate>> GetKeyPhraseRelates(int id);

        Task<ResponceMessage> AddKeyphrase(String keyphrase);

        Task<ResponceMessage> DeleteKeyphrase(int id);

    }
}
