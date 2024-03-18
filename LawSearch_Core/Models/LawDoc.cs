using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class LawDoc
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }

    public class LawHTML
    {
        public int? LawID { get; set; } = null;
        public string contentHTML { get; set; }
    }

}
