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

        if (id != null)
        {
            ViewData["InstructorID"] = id;
            Instructor instructor = viewModel.Instructors.Single(i => i.ID == id);
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

        var instructor = await _context.Instructors.Include(i => i.OfficeAssignment)
            .FirstOrDefaultAsync(m => m.ID == id);
        if (instructor == null)
        {
            return NotFound();
        }

        return View(instructor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string officeLocation)
    {
        var instructor = await _context.Instructors.Include(i => i.OfficeAssignment)
            .FirstOrDefaultAsync(i => i.ID == id);
        if (instructor == null)
        {
            return NotFound();
        }

        if (String.IsNullOrWhiteSpace(officeLocation))
        {
            //If the office location is empty the OfficeAssignment object associated with the instructor must be set to null
            instructor.OfficeAssignment = null;
        }
        else if (instructor.OfficeAssignment == null)
        {
            //If OfficeAssignment == null
            //Create a new OfficeAssignment
            //Assign that to the Instructor.
            var officeAssignment = new OfficeAssignment();
            officeAssignment.Instructor = instructor;
            officeAssignment.InstructorID = instructor.ID;
            officeAssignment.Location = officeLocation;
            _context.OfficeAssignments.Add(officeAssignment);
            instructor.OfficeAssignment = officeAssignment;
        }
        else
        {
            //Just update the location for the instructor's existing OfficeAssignment
            instructor.OfficeAssignment.Location = officeLocation;
        }

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

    // [HttpPost, ActionName("Edit")]
    // public async Task<IActionResult> EditPost(int? id)
    // {
    //     if (id == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var instructorToUpdate = await _context.Instructors
    //         .Include(i => i.OfficeAssignment)
    //         .FirstOrDefaultAsync(s => s.ID == id);
    //
    //     //Somehow TryUpdateModelAsync gets access to the office location submitted in the form,
    //     //however, if the OfficeAssignment of an Instructor is already empty/null, it won't be able to update. BRUH 
    //     bool updateOk = await TryUpdateModelAsync<Instructor>(instructorToUpdate, "", i => i.FirstMidName,
    //         i => i.LastName,
    //         i => i.HireDate, i => i.OfficeAssignment);
    //     if (updateOk)
    //     {
    //         //if the office location was empty in the form, set the instructor's OfficeAssignment prop to null,
    //         //since the previous obj is not longer required.
    //         if (string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
    //         {
    //             instructorToUpdate.OfficeAssignment = null;
    //         }
    //
    //         try
    //         {
    //             await _context.SaveChangesAsync();
    //         }
    //         catch (DbUpdateException e)
    //         {
    //             ModelState.AddModelError("", "Unable to save changes. " +
    //                                          "Try again, and if the problem persists, " +
    //                                          "see your system administrator.");
    //         }
    //
    //         return RedirectToAction(nameof(Index));
    //     }
    //
    //     return View(instructorToUpdate);
    // }

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