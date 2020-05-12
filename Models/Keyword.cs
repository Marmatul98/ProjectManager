using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManager.Models
{
    public class Keyword
    {
        public int KeywordID { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<ProjectKeyword> ProjectKeywords { get; set; }
    }
}
