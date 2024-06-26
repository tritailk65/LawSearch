﻿using LawSearch_Core.Models;
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
        List<ConceptKeyphraseShow> GetConceptKeyphraseByConceptID(int id);
        Task GenerateKeyphraseDescript(int id);
        void AddConceptKeyphrase(int concept_id, string keyphrase);
        void GenerateConceptMapping(int LawID);
        void DeleteConceptKeyphrase(int KeyphraseID);
        void DeleteConceptMapping(int LawID);
    }
}
