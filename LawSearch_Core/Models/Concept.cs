using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class Concept
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int type { get; set; }
        public double[] vector;
        public double distance;


    }
}
