using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContosoCandies.Data.Interfaces;
using ContosoCandies.Data.Models;

namespace ContosoCandies.Data.Services
{
    public class CandieService : ICandieService
    {
        private ContosoCandiesContext _context;

        public CandieService(ContosoCandiesContext context)
        {
            _context = context;
        }

        public List<Candies> GetAllCandies()
        {
            List<Candies> candies = _context.Candies.ToList();
            return candies;
        }
    }
}
