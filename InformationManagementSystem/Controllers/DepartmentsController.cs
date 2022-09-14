using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InformationManagementSystem.Data;
using InformationManagementSystem.Models;

namespace InformationManagementSystem.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly SchoolContext _context;

        public DepartmentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.Departments.Include(d => d.Administrator);
            return View(await schoolContext.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Administrator)
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FullName");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,Name,Budget,StartDate,InstructorID")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.
                Include(d => d.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //TODO: Fix this API
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var departmentToUpdate = await _context.Departments
        //         .Include(d => d.Administrator)
        //         .FirstOrDefaultAsync(d => d.DepartmentID == id);
        //
        //     if (departmentToUpdate == null)
        //     {
        //         //This means some other user has deleted the department
        //         Department deletedDepartment = new Department();
        //         await TryUpdateModelAsync(deletedDepartment);
        //         ModelState.AddModelError("", "Unable to save changes. The department was deleted by another user.");
        //         ViewData["InstructorID"] =
        //             new SelectList(_context.Instructors, "ID", "FullName", deletedDepartment.DepartmentID);
        //         return View(deletedDepartment);
        //     }
        //
        //     // The view stores the original RowVersion value in a hidden field, and this method receives that value in the rowVersion parameter. 
        //     
        //     // Before you call SaveChanges, you have to put that original RowVersion property value in the OriginalValues collection for the entity.
        //     
        //     // Then when the Entity Framework creates a SQL UPDATE command, that command will include a WHERE clause that looks for a row that has the original RowVersion value.
        //      // If no rows are affected by the UPDATE command (no rows have the original RowVersion value), the Entity Framework throws a DbUpdateConcurrencyException exception.
        //     _context.Entry(departmentToUpdate).Property("RowVersion").OriginalValue = rowVersion;
        //
        //     var validationErrors = ModelState.Values.Where(E => E.Errors.Count > 0)
        //         .SelectMany(E => E.Errors)
        //         .Select(E => E.ErrorMessage)
        //         .ToList();
        //     
        //     if (await TryUpdateModelAsync<Department>(
        //             departmentToUpdate,
        //             "",
        //             s => s.Name, s => s.StartDate, s => s.Budget, s => s.InstructorID))
        //     {
        //         try
        //         {
        //             await _context.SaveChangesAsync();
        //             return RedirectToAction(nameof(Index));
        //         }
        //         catch (DbUpdateConcurrencyException e)
        //         {
        //             var exceptionEntry = e.Entries.Single();
        //             var clientValues = (Department)exceptionEntry.Entity;
        //             var databaseEntry = exceptionEntry.GetDatabaseValues();
        //             if (databaseEntry == null)
        //             {
        //                 ModelState.AddModelError("",
        //                     "Unable to save changes. The department was deleted by another user.");
        //             }
        //             else
        //             {
        //                 var databaseValues = (Department)databaseEntry.ToObject();
        //
        //                 if (databaseValues.Name != clientValues.Name)
        //                 {
        //                     ModelState.AddModelError("Name", $"Current value: {databaseValues.Name}");
        //                 }
        //                 if (databaseValues.Budget != clientValues.Budget)
        //                 {
        //                     ModelState.AddModelError("Budget", $"Current value: {databaseValues.Budget}");
        //                 }
        //                 if (databaseValues.StartDate != clientValues.StartDate)
        //                 {
        //                     ModelState.AddModelError("StartDate", $"Current value: {databaseValues.StartDate}");
        //                 }
        //                 if (databaseValues.InstructorID != clientValues.InstructorID)
        //                 {
        //                     Instructor databaseInstructor =
        //                         await _context.Instructors.FirstOrDefaultAsync(i => i.ID == databaseValues.InstructorID);
        //                     ModelState.AddModelError("InstructorID", $"Current value: {databaseInstructor.FullName}");
        //                 }
        //
        //                 ModelState.AddModelError(string.Empty, "The record you attempted to edit "
        //                                                        + "was modified by another user after you got the original value. The "
        //                                                        + "edit operation was canceled and the current values in the database "
        //                                                        + "have been displayed. If you still want to edit this record, click "
        //                                                        + "the Save button again. Otherwise click the Back to List hyperlink.");
        //                 // Finally, the code sets the RowVersion value of the departmentToUpdate to the new value retrieved from the database.    
        //                 // This new RowVersion value will be stored in the hidden field when the Edit page is redisplayed, and the next time the user clicks Save,
        //                 // only concurrency errors that happen since the redisplay of the Edit page will be caught.
        //                 departmentToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
        //
        //                 // The ModelState.Remove statement is required because ModelState has the old RowVersion value. In the view, the ModelState value for a field takes precedence over the model property values when both are present.
        //                 ModelState.Remove("RowVersion");
        //
        //             }
        //         }
        //     }
        //     ViewData["InstructorID"] =
        //         new SelectList(_context.Instructors, "ID", "FullName", departmentToUpdate.DepartmentID);
        //     return View(departmentToUpdate);
        // }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("DepartmentID,Name,Budget,StartDate,InstructorID")] Department department)
        {
            if (id != department.DepartmentID)
            {
                return NotFound();
            }

            var departmentToUpdate = await _context.Departments
                .Include(d => d.Administrator)
                .Include(d => d.Courses)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DepartmentID == id);
            if (departmentToUpdate == null)
            {
                return NotFound();
            }

            departmentToUpdate.Name = department.Name;
            departmentToUpdate.Budget = department.Budget;
            departmentToUpdate.StartDate = department.StartDate;

            if (departmentToUpdate.InstructorID != department.InstructorID)
            {
                //This means the instructor has changed
                departmentToUpdate.InstructorID = department.InstructorID;
                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.ID == department.InstructorID);
                departmentToUpdate.Administrator = instructor;
            }

            // ModelState.Remove("RowVersion");
            ModelState.Remove("Courses");
            ModelState.Remove("Administrator");
            
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(departmentToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Administrator)
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Departments == null)
            {
                return Problem("Entity set 'SchoolContext.Departments'  is null.");
            }
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
          return (_context.Departments?.Any(e => e.DepartmentID == id)).GetValueOrDefault();
        }
    }
}
