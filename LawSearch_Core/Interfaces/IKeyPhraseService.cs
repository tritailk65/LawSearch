﻿using LawSearch_Core.Models;
using System.Data;

namespace LawSearch_Core.Interfaces
{
    public interface IKeyPhraseService
    {
        List<KeyPhrase> GetListKeyPhrase();
        List<KeyPhraseRelate> GetKeyPhraseRelateDetailsByID(int ID);
        KeyPhrase AddKeyPhrase(KeyPhrase keyPhrase);
        void DeleteKeyPhrase(int id);
        Task GenerateKeyphraseVNCoreNLP(int LawID);
        void GenerateKeyphraseMapping(int LawID);
        void DeleteKeyPhraseMapping(int KeyphraseID, int ArticalID);
        void DeleteAllKeyPhraseMapping(int LawID);
    }
}
