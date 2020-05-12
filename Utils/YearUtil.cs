using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManager.Utils
{
    public class YearUtil
    {
        private static int currentYear = int.Parse(DateTime.Now.Year.ToString());

        public static IEnumerable<int> GetAvailableYearsRange()
        {
            return Enumerable.Range(currentYear - 2, 5);
        }

        public static SelectList GetSelectListOfAvailableYears()
        {
            return new SelectList(GetAvailableYearsRange());
        } 
    }
}
