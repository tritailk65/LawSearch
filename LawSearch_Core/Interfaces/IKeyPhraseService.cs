using LawSearch_Core.Models;
using System.Data;

namespace LawSearch_Core.Interfaces
{
    public interface IKeyPhraseService
    {
        List<KeyPhrase> GetListKeyPhrase();
        List<KeyPhraseRelate> GetKeyPhraseRelateDetailsByID(int ID);
        KeyPhrase AddKeyPhrase(KeyPhrase keyPhrase);
        void DeleteKeyPhrase(int id);
        void GenerateKeyPhraseMapping(int LawID);
        void DeleteAllKeyphraseMapping();
        void DeleteKeyphraseMapping(int KeyphraseID);
    }
}
