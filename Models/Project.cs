using ProjectManager.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManager.Models
{
    public class Project
    {
        public int ProjectID { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Student")]
        public int StudentID { get; set; }

        public Student Student { get; set; }

        [Required]
        public int Year { get; set; }

        public Course Course { get; set; }

        public ICollection<ProjectKeyword> ProjectKeywords { get; set; }

        [Display(Name = "Documentation")]
        public string DocumentationPath { get; set; }

        public string PresentationPath { get; set; }

        public string ProjectPath { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Project project &&
                   ProjectID == project.ProjectID &&
                   Title == project.Title &&
                   Description == project.Description &&
                   StudentID == project.StudentID &&
                   EqualityComparer<Student>.Default.Equals(Student, project.Student) &&
                   Year == project.Year &&
                   Course == project.Course &&
                   EqualityComparer<ICollection<ProjectKeyword>>.Default.Equals(ProjectKeywords, project.ProjectKeywords) &&
                   DocumentationPath == project.DocumentationPath &&
                   PresentationPath == project.PresentationPath &&
                   ProjectPath == project.ProjectPath;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(ProjectID);
            hash.Add(Title);
            hash.Add(Description);
            hash.Add(StudentID);
            hash.Add(Student);
            hash.Add(Year);
            hash.Add(Course);
            hash.Add(ProjectKeywords);
            hash.Add(DocumentationPath);
            hash.Add(PresentationPath);
            hash.Add(ProjectPath);
            return hash.ToHashCode();
        }
    }
}