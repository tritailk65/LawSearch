using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Interfaces
{
    public interface IConceptService
    {
        List<Concept> GetListConcept();
        Concept AddConcept(Concept concept);
        Concept UpdateConcept(Concept concept);
        void DeleteConcept(int id);
        List<KeyPhrase> GetKeyPhrasesFormConceptID(int id);
        Task GenerateKeyphraseDescript(int LawID);
        void AddConceptKeyphrase(int concept_id, string keyphrase);
        void GenerateConceptMapping(int LawID);
        void GenerateMappingFromName(int LawID);
        void DeleteConceptKeyphrase(int KeyphraseID);
    }
}
