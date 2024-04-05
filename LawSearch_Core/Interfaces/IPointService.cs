using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Interfaces
{
    public interface IPointService
    {
        List<Point> GetListPointByLawID(int lawID);
    }
}
