using InformationManagementSystem.Data;
using InformationManagementSystem.Models;
using InformationManagementSystem.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationManagementSystem.Controllers;

public class InstructorsController : Controller
{
    private SchoolContext _context;

    public InstructorsController(SchoolContext context)
    {
        _context = context;
    }

    //GET Instructors
    public async Task<IActionResult> Index(int? id, int? courseID)
    {
        var viewModel = new InstructorIndexData();
        viewModel.Instructors = await _context.Instructors
            .Include(i => i.OfficeAssignment)
            .Include(i => i.CourseAssignments)
                .ThenInclude(i => i.Course)
                    .ThenInclude(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
            .Include(i => i.CourseAssignments)
                .ThenInclude(i => i.Course)
                    .ThenInclude(c => c.Department)
            .AsNoTracking()
            .OrderBy(i => i.LastName)
            .ToListAsync();

        if (id!=null)
        {
            ViewData["InstructorID"] = id;
            Instructor instructor = viewModel.Instructors.Single(i => i.ID==id);
            viewModel.Courses = instructor.CourseAssignments.Select(ca => ca.Course);
        }

        if (courseID != null)
        {
            ViewData["CourseID"] = courseID;
            viewModel.Enrollments = viewModel.Courses.Single(c => c.CourseID == courseID).Enrollments;
        }
        
        return View(viewModel);
    }

    //GET Instructors/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Instructors == null)
        {
            return NotFound();
        }

        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor == null)
        {
            return NotFound();
        }

        return View(instructor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ID, LastName, FirstMidName,HireDate")] Instructor instructor)
    {
        if (id != instructor.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(instructor);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e);
                throw;
            }

            return RedirectToAction("Index");
        }

        return View(instructor);
    }

    // GET: Instructors/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Instructors == null)
        {
            return NotFound();
        }

        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.ID == id);
        if (instructor == null)
        {
            return NotFound();
        }

        return View(instructor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var instructor = await _context.Instructors.FindAsync(id);

        //This would have been required if ON DELETE CASCADE hadn't been the default
        // Instructor instructor = await _context.Instructors
        //     .Include(i => i.CourseAssignments)
        //     .SingleAsync(i => i.ID == id);

        // If the instructor to be deleted is assigned as administrator of any departments, removes the instructor assignment from those departments.
        var departments = await _context.Departments.Where(d => d.InstructorID == id).ToListAsync();
        departments.ForEach(d => d.InstructorID = null);

        if (instructor != null)
        {
            try
            {
                _context.Instructors.Remove(instructor);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        return RedirectToAction(nameof(Index));
    }
}