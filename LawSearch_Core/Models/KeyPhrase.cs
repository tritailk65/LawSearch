using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class KeyPhrase
    {
        public int ID { get; set; }
        public string Keyphrase {  get; set; }
        public string Key { get; set; }
        public KeyPhraseSource Source { get; set; }
        public int? ConceptID { get; set; } = null;
        public int? NumberArtical { get; set; } = null;
        public string? KeyNorm { get; set; }
        public int? LawID { get; set; } = null;
        public int Count { get; set; }

    }

    public class KeyPhraseRelate
    {
        public int KeyPhraseID { get; set; }
        public string KeyPhrase { get; set; }
        public string ArticalName { get; set; }
        public int ArticalID { get; set; }
        public int NumCount { get; set; }
        public string ChapterName { get; set; }
    }

    public enum KeyPhraseSource
    {
        Auto = 0,
        Manual = 1,
        Search = 2
    }
}
