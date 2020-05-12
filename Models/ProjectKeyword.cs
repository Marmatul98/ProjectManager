using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManager.Models
{
    public class ProjectKeyword
    {
        public int ProjectKeywordID { get; set; }

        public int KeywordID { get; set; }

        public int ProjectID { get; set; }

        public Project Project { get; set; }

        public Keyword Keyword { get; set; }
    }
}
