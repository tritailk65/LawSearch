using System.Data;

namespace LawSearch_Core.Interfaces
{
    public interface IKeyPhraseService
    {
        DataTable GetListKeyPhrase();
        DataTable GetKeyPhraseRelateDetailsByID(int ID);
    }
}
