using InformationManagementSystem.Data;
using InformationManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationManagementSystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        // ASP .Net Core dependency injection takes care of passing an instance of SchoolContext into controller.
        // We configured this by registering SchoolContext in Program.cs
        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder,
            string searchStringBabaYaga,
            string currentFilter,
            int? pageNumber)
        {
            //sortOrder comes from query string URL, provided by ASP.NET MVC as a param to action method
            // The first time the index page is requested, sortOrder will be empty string

            // The two ViewData elements (NameSortParm and DateSortParm) are used by the view to configure the column heading hyperlinks with the appropriate query string values.
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "Date" ? "date_desc" : "Date";
            // This is sorta toggle mechanism


            ViewData["SortOrder"] = sortOrder; //maintain sortOrder at all times (even when paging).

            // This piece of code maintains the searchString at all times
            if (searchStringBabaYaga != null)
            {
                //the user pressed the Search button
                //Therefore, the current filter must be updated with the new search string provided by the user
                currentFilter = searchStringBabaYaga;
            }
            else
            {
                //The searchString should be same as the already set filter  
                searchStringBabaYaga = currentFilter;
            }
            //
            
            ViewData["CurrentFilter"] = searchStringBabaYaga;

            IQueryable<Student> students = from s in _context.Students select s;

            //check if user is searching for a student 
            if (!string.IsNullOrEmpty(searchStringBabaYaga))
            {
                students = students.Where(s =>
                    s.FirstMidName!.Contains(searchStringBabaYaga) || s.LastName!.Contains(searchStringBabaYaga));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;
            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Students
        // public async Task<IActionResult> Index()
        // {
        //       return _context.Students != null ? 
        //                   View(await _context.Students.ToListAsync()) :
        //                   Problem("Entity set 'SchoolContext.Students'  is null.");
        // }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            // NOTE: Scaffolded code for Students Index page will leave Enrollments because that property holds a collection. Therefore it must be included below StudentsController Details() method
            var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .AsNoTracking() //AsNoTracking method improves performance in scenarios where the entities returned won't be updated in the current context's lifetime.
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] //helps prevent cross-site request forgery(CSRF) attack
        public async Task<IActionResult> Create([Bind("LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            //Bind attribute limits the fields read from API request. This prevents adding additional malicious code/info. 
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            if (id != student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                //NOTE: DB concurrency violation may occur. That is, two different users try to update the same row
                //at the same time, and everything will blow up. 
                catch (DbUpdateException)
                {
                    if (!StudentExists(student.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. " +
                                                     "Try again, and if the problem persists, " +
                                                     "see your system administrator.");
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'SchoolContext.Students'  is null.");
            }

            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return (_context.Students?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}