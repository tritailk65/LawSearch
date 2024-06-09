using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface IKeyPhraseAdminService
    {
        Task<List<KeyPhrase>> GetListKeyPhrase();

        Task<List<KeyPhraseRelate>> GetKeyPhraseRelates(int id);

        Task<ResponseMessage> AddKeyphrase(String keyphrase);

        Task<ResponseMessage> DeleteKeyphrase(int id);

        Task<ResponseMessage> DeleteKeyphraseMapping(int KeyphraseID, int ArticalID);

        Task<bool> DeleteAllKeyphraseMapping(int LawID);
    }
}
