using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManager.Enums
{
    public enum Course
    {
        APLUI,
        UMINT,
        SOFTCO
    }

    public static class CourseMethods
    {
        public static IEnumerable<String> GetCoursesAsStringList()
        {
            return Enum.GetNames(typeof(Course));
        }
        
        public static Course GetCourseByName(string courseName)
        {
            return (Course) Enum.Parse(typeof(Course), courseName, true);
        }
    }
}
