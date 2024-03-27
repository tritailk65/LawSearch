using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class KeyPhraseResult
    {
        public List<KeyPhrase> keys;

        public int ID { get; set; }
        public double distance { get; set; }
        public double[] vector { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string LawName { get; set; }

        public KeyPhraseResult() { keys = new List<KeyPhrase>(); }
    }
}
