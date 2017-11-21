using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContosoCandies.Data.Models;

namespace ContosoCandies.Data.Interfaces
{
    public interface ICandieService
    {
        List<Candies> GetAllCandies();
    }
}
