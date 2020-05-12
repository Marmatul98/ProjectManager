using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Enums;
using ProjectManager.Models;
using ProjectManager.ViewModels;

namespace ProjectManager.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ManagerContext _context;
        private readonly IHostingEnvironment hostingEnvironment;

        public ProjectsController(ManagerContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> List(string courseName, string[] selectedKeywords)
        {
            ViewData["CourseName"] = courseName;

            var projects = await _context.Projects
                .Include(p => p.Student)
                .Where(p => p.Course == CourseMethods.GetCourseByName(courseName)).ToListAsync();

            ISet<Project> displayedProjects = new HashSet<Project>();


            if (!selectedKeywords.Any())
            {
                displayedProjects = projects.ToHashSet();
                ViewData["FilterKeywords"] = GetAvailableKeywordsToFilter(projects, new string[] { });
            }
            else
            {
                foreach (var project in projects)
                {
                    foreach (var keyword in GetProjectKeywordsFromProject(project))
                    {
                        if (selectedKeywords.ToList().Contains(keyword.KeywordID.ToString()))
                        {
                            displayedProjects.Add(project);
                        }
                    }
                }
                ViewData["FilterKeywords"] = GetAvailableKeywordsToFilter(projects, selectedKeywords);
            }
            ViewData["Projects"] = projects;
            return View(displayedProjects);
        }

        private ICollection<AssignedKeywordData> GetAvailableKeywordsToFilter(List<Project> projects, string[] selectedKeywords)
        {
            ISet<AssignedKeywordData> returnedSet = new HashSet<AssignedKeywordData>();
            foreach (var project in projects)
            {
                foreach (var projectKeyword in GetProjectKeywordsFromProject(project))
                {
                    Keyword keywordObject = GetKeywordFromProjectKeyword(projectKeyword);
                    AssignedKeywordData assignedKeyword = new AssignedKeywordData { Name = keywordObject.Name, KeywordID = keywordObject.KeywordID };
                    assignedKeyword.Assigned = selectedKeywords.Contains(projectKeyword.KeywordID.ToString());
                    returnedSet.Add(assignedKeyword);
                }
            }
            return returnedSet;
        }

        private Keyword GetKeywordFromProjectKeyword(ProjectKeyword projectKeyword)
        {
            var keyword = _context.Keywords.Where(k => k.KeywordID == projectKeyword.KeywordID).FirstOrDefault();
            return keyword;
        }

        private ICollection<ProjectKeyword> GetProjectKeywordsFromProject(Project project)
        {
            var keywords = _context.ProjectKeywords.Where(k => k.ProjectID == project.ProjectID).ToList();
            return keywords;
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Student)
                .FirstOrDefaultAsync(m => m.ProjectID == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize]
        public IActionResult Create(string fromCourse)
        {
            ViewData["Course"] = fromCourse;
            var project = new Project();
            project.ProjectKeywords = new List<ProjectKeyword>();
            PopulateAssignedKeywordData(project);
            ViewData["StudentNames"] = new SelectList(_context.Students, "StudentID", "Name");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Title,Description,StudentID,Year,Course,Documentation,Presentation,Project")] ProjectCreateViewModel projectViewModel, string[] selectedKeywords)
        {
            Project project = new Project();
            project.Course = projectViewModel.Course;
            project.Title = projectViewModel.Title;
            project.Description = projectViewModel.Description;
            project.Year = projectViewModel.Year;
            project.StudentID = projectViewModel.StudentID;

            if (selectedKeywords != null)
            {
                project.ProjectKeywords = new List<ProjectKeyword>();
                foreach (var keyword in selectedKeywords)
                {
                    var keywordToAdd = new ProjectKeyword { ProjectID = project.ProjectID, KeywordID = int.Parse(keyword) };
                    project.ProjectKeywords.Add(keywordToAdd);
                }
            }

            string uniqueDocumentationFileName = null;
            string uniquePresentationFileName = null;
            string uniqueProjectFileName = null;
            if (ModelState.IsValid)
            {
                if (projectViewModel.Documentation != null)
                {
                    string uploadDocumentationFolder = Path.Combine(hostingEnvironment.WebRootPath, "Documentation");
                    uniqueDocumentationFileName = Guid.NewGuid().ToString() + "_" + projectViewModel.Documentation.FileName;
                    string documentationFilePath = Path.Combine(uploadDocumentationFolder, uniqueDocumentationFileName);

                    using (var stream = new FileStream(documentationFilePath, FileMode.Create))
                    {
                        projectViewModel.Documentation.CopyTo(stream);
                    }

                }
                if (projectViewModel.Presentation != null)
                {
                    string uploadPresentationFolder = Path.Combine(hostingEnvironment.WebRootPath, "Presentation");
                    uniquePresentationFileName = Guid.NewGuid().ToString() + "_" + projectViewModel.Presentation.FileName;
                    string presentationFilePath = Path.Combine(uploadPresentationFolder, uniquePresentationFileName);

                    using (var stream = new FileStream(presentationFilePath, FileMode.Create))
                    {
                        projectViewModel.Presentation.CopyTo(stream);
                    }

                }
                if (projectViewModel.Project != null)
                {
                    string uploadProjectFolder = Path.Combine(hostingEnvironment.WebRootPath, "Project");
                    uniqueProjectFileName = Guid.NewGuid().ToString() + "_" + projectViewModel.Project.FileName;
                    string projectFilePath = Path.Combine(uploadProjectFolder, uniqueProjectFileName);

                    using (var stream = new FileStream(projectFilePath, FileMode.Create))
                    {
                        projectViewModel.Project.CopyTo(stream);
                    }

                }
                project.DocumentationPath = uniqueDocumentationFileName;
                project.PresentationPath = uniquePresentationFileName;
                project.ProjectPath = uniqueProjectFileName;
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction("List", "Projects", new { courseName = project.Course.ToString() });
            }
            ViewData["StudentNames"] = new SelectList(_context.Students, "StudentID", "Name", project.StudentID);
            PopulateAssignedKeywordData(project);
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.ProjectKeywords).ThenInclude(p => p.Keyword)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ProjectID == id);

            if (project == null)
            {
                return NotFound();
            }
            ViewData["StudentNames"] = new SelectList(_context.Students, "StudentID", "Name", project.StudentID);
            PopulateAssignedKeywordData(project);
            return View(project);
        }

        private void PopulateAssignedKeywordData(Project project)
        {
            var allKeywords = _context.Keywords;
            var projectKeywords = new HashSet<int>(project.ProjectKeywords.Select(k => k.KeywordID));
            var viewModel = new List<AssignedKeywordData>();
            foreach (var keyword in allKeywords)
            {
                viewModel.Add(new AssignedKeywordData
                {
                    KeywordID = keyword.KeywordID,
                    Name = keyword.Name,
                    Assigned = projectKeywords.Contains(keyword.KeywordID)
                });
            }
            ViewData["Keywords"] = viewModel;
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int? id, string[] selectedKeywords)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectToUpdate = await _context.Projects
                .Include(p => p.ProjectKeywords)
                .ThenInclude(p => p.Keyword)
                .FirstOrDefaultAsync(m => m.ProjectID == id);

            if (await TryUpdateModelAsync<Project>(
                projectToUpdate,
                "",
                p => p.Title, p => p.Description, p => p.Course, p => p.StudentID, p => p.Year, p => p.ProjectKeywords))
            {
                UpdateProjectKeywords(selectedKeywords, projectToUpdate);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                return RedirectToAction("List", "Projects", new { courseName = projectToUpdate.Course.ToString() });
            }
            ViewData["StudentNames"] = new SelectList(_context.Students, "StudentID", "Name", projectToUpdate.StudentID);
            UpdateProjectKeywords(selectedKeywords, projectToUpdate);
            PopulateAssignedKeywordData(projectToUpdate);
            return View(projectToUpdate);
        }


        private void UpdateProjectKeywords(string[] selectedKeywords, Project projectToUpdate)
        {
            if (selectedKeywords == null)
            {
                projectToUpdate.ProjectKeywords = new List<ProjectKeyword>();
                return;
            }

            var selectedKeywordsHS = new HashSet<string>(selectedKeywords);
            var projectKeywords = new HashSet<int>
                (projectToUpdate.ProjectKeywords.Select(k => k.Keyword.KeywordID));
            foreach (var keyword in _context.Keywords)
            {
                if (selectedKeywordsHS.Contains(keyword.KeywordID.ToString()))
                {
                    if (!projectKeywords.Contains(keyword.KeywordID))
                    {
                        projectToUpdate.ProjectKeywords.Add(new ProjectKeyword { ProjectID = projectToUpdate.ProjectID, KeywordID = keyword.KeywordID });
                    }
                }
                else
                {

                    if (projectKeywords.Contains(keyword.KeywordID))
                    {
                        ProjectKeyword keywordToRemove = projectToUpdate.ProjectKeywords.FirstOrDefault(p => p.KeywordID == keyword.KeywordID);
                        _context.Remove(keywordToRemove);
                    }
                }
            }
        }

        // GET: Projects/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Student)
                .FirstOrDefaultAsync(m => m.ProjectID == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction("List", "Projects", new { courseName = project.Course.ToString() });
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectID == id);
        }

        public FileResult DownloadFile(string fileName, string studentName, string fileType)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileType, fileName);
            return File(new FileStream(filePath, FileMode.Open), GetContentType(filePath), fileType + studentName + Path.GetExtension(filePath).ToLowerInvariant());
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".zip", "application/zip"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
