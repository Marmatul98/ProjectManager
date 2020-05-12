using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManager.ViewModels
{
    public class AssignedKeywordData
    {
        public int KeywordID { get; set; }
        public string Name { get; set; }
        public bool Assigned { get; set; }

        public override bool Equals(object obj)
        {
            return obj is AssignedKeywordData data &&
                   KeywordID == data.KeywordID &&
                   Name == data.Name &&
                   Assigned == data.Assigned;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(KeywordID, Name, Assigned);
        }
    }
}
