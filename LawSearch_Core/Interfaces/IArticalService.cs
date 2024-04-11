using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Interfaces
{
    public interface IArticalService
    {
        List<Artical> GetAllArtical();
        List<Artical> GetListArticalByLawID(int lawID);
    }
}
