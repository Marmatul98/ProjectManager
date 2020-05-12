using ProjectManager.Enums;
using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManager.Data
{
    public class DbInitializer
    {
        public static void Initialize(ManagerContext context)
        {          
            context.Database.EnsureCreated();     
            if (context.Students.Any())
            {
                return;   // DB has been seeded
            }

            var students = new Student[]
            {
            new Student{FirstName="Carson",LastName="Alexander", Email="carsona@gmail.com", PersonalNumber="R18423"},
            new Student{FirstName="Meredith",LastName="Alonso",Email="mereditha@gmail.com", PersonalNumber="R18753"},
            new Student{FirstName="Arturo",LastName="Anand",Email="arturoa@gmail.com", PersonalNumber="R18469"},
            };
            foreach (Student s in students)
            {
                context.Students.Add(s);
            }
            context.SaveChanges();

            var projects = new Project[]
            {
            new Project{StudentID=1, Course=Course.APLUI, Title="ProjectTitleAplui", Description="ProjectDescriptionAplui", Year=2019},
            new Project{StudentID=2,Course =Course.SOFTCO, Title="ProjectTitleSoftco", Description="ProjectDescriptionSoftco", Year=2020},
            new Project{ StudentID=3,Course=Course.UMINT, Title="ProjectTitleUmint", Description="ProjectDescriptionUmint", Year=2021},
             new Project{ StudentID=1,Course=Course.UMINT, Title="ProjectTitleUmint2", Description="ProjectDescriptionUmint2", Year=2022},
              new Project{ StudentID=2,Course=Course.UMINT, Title="ProjectTitleUmint3", Description="ProjectDescriptionUmint3", Year=2020},
               new Project{ StudentID=1,Course=Course.UMINT, Title="ProjectTitleUmint4", Description="ProjectDescriptionUmint4", Year=2021},
                new Project{ StudentID=3,Course=Course.UMINT, Title="ProjectTitleUmint5", Description="ProjectDescriptionUmint5", Year=2019}
            };

            foreach (Project p in projects)
            {
                context.Projects.Add(p);
            }
            context.SaveChanges();

            var keywords = new Keyword[]
            {
                new Keyword{Name="Car"},
                new Keyword{Name="Plane"},
                new Keyword{Name="Food"},
            };

            foreach (Keyword k in keywords)
            {
                context.Keywords.Add(k);
            }
            context.SaveChanges();

            var projectKeywords = new ProjectKeyword[]
            {
            new ProjectKeyword{ProjectID=1,KeywordID=1},
            new ProjectKeyword{ProjectID=1,KeywordID=3},
            new ProjectKeyword{ProjectID=2,KeywordID=2},
            new ProjectKeyword{ProjectID=2,KeywordID=3},
            new ProjectKeyword{ProjectID=3,KeywordID=1},
            new ProjectKeyword{ProjectID=4,KeywordID=2},
            new ProjectKeyword{ProjectID=5,KeywordID=3},
            new ProjectKeyword{ProjectID=6,KeywordID=2},
            new ProjectKeyword{ProjectID=7,KeywordID=1},
            };
            foreach (ProjectKeyword p in projectKeywords)
            {
                context.ProjectKeywords.Add(p);
            }
            context.SaveChanges();
        }
    }
}

