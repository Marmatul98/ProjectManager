using Microsoft.AspNetCore.Http;
using ProjectManager.Enums;
using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManager.ViewModels
{
    public class ProjectCreateViewModel
    {
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

        [Display(Name = "Keywords")]
        public ICollection<ProjectKeyword> ProjectKeywords { get; set; }

        public IFormFile Documentation { get; set; }

        public IFormFile Presentation { get; set; }

        public IFormFile Project { get; set; }
    }
}
