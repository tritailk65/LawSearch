using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class Point
    {
        public int ID { get; set; }
        public int ClauseID { get; set; }
        public string Content { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }

        internal string GetPointName(int number)
        {
            string[] arr = new string[] { "", "a)", "b)", "c)", "d)", "đ)", "e)", "g)", "h)", "i)", "k)", "l)", "m)", "n)", "o)", "p)", "q)", "r)", "s)", "t)", "u)", "v)", "x)", "y)" };
            return arr.Length > number ? arr[number] : "";
        }
    }
}
