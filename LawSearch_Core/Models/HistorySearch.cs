using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class HistorySearch
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public string Result { get; set; }
        public DateTime DateTime { get; set; }
        public string UserName { get; set; }
        public int Count { get; set; }  
    }
    
}
