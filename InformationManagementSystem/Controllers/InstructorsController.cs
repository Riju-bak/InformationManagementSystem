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
            .Include(i => i.CourseAssignments)
            .ThenInclude(ca => ca.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);
        if (instructor == null)
        {
            return NotFound();
        }

        PopulateAssignedCourseData(instructor);
        return View(instructor);
    }

    private void PopulateAssignedCourseData(Instructor instructor)
    {
        var allCourses = _context.Courses;
        var instructorCourses = new HashSet<int>(instructor.CourseAssignments.Select(ca => ca.CourseID));
        var viewModel = new List<AssignedCourseData>();
        foreach (var course in allCourses)
        {
            viewModel.Add(new AssignedCourseData
            {
                CourseID = course.CourseID,
                Title = course.Title,
                Assigned = instructorCourses.Contains(course.CourseID)
            });
        }

        ViewData["Courses"] = viewModel;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string officeLocation, string[] selectedCourses)
    {
        var instructor = await _context.Instructors.Include(i => i.OfficeAssignment)
            .Include(i => i.CourseAssignments)
            .ThenInclude(ca => ca.Course)
            .FirstOrDefaultAsync(i => i.ID == id);
        if (instructor == null)
        {
            return NotFound();
        }

        UpdateInstructorOffice(officeLocation, instructor);
        UpdateInstructorCourse(selectedCourses, instructor);

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

    private void UpdateInstructorCourse(string[] selectedCourses, Instructor instructor)
    {
        if (selectedCourses == null || selectedCourses.Length == 0)
        {
            instructor.CourseAssignments = new List<CourseAssignment>();
        }

        var instructorCourses = new HashSet<int>(instructor.CourseAssignments.Select(ca => ca.CourseID));
        var selectedCoursesHS = new HashSet<string>(selectedCourses);
        foreach (var course in _context.Courses)
        {
            if (selectedCoursesHS.Contains(course.CourseID.ToString()))
            {
                if (!instructorCourses.Contains(course.CourseID))
                {
                    instructor.CourseAssignments.Add(new CourseAssignment{Course = course, CourseID = course.CourseID, Instructor = instructor, InstructorID = instructor.ID});
                }
            }
            else
            {
                if (instructorCourses.Contains(course.CourseID))
                {
                    CourseAssignment coursesToRemove = instructor.CourseAssignments.FirstOrDefault(i => i.CourseID == course.CourseID);
                    _context.Remove(coursesToRemove);
                }
            }
        }
        
    }

    private void UpdateInstructorOffice(string officeLocation, Instructor instructor)
    {
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
            var officeAssignment = new OfficeAssignment
            {
                Instructor = instructor,
                InstructorID = instructor.ID,
                Location = officeLocation
            };
            _context.OfficeAssignments.Add(officeAssignment);
            instructor.OfficeAssignment = officeAssignment;
        }
        else
        {
            //Just update the location for the instructor's existing OfficeAssignment
            instructor.OfficeAssignment.Location = officeLocation;
        }
    }

    //THE IMPLEMENTATION BELOW IS COMMENTED SINCE AT HIS SOME PROBLEMS
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

    public IActionResult Create()
    {
        var instructor = new Instructor();
        instructor.CourseAssignments = new List<CourseAssignment>();
        PopulateAssignedCourseData(instructor);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstMidName", "LastName", "HireDate", "OfficeAssignment")] Instructor instructor, string? officeLocation, string[] selectedCourses)
    {
        if (selectedCourses != null || selectedCourses.Length > 0)
        {
            instructor.CourseAssignments = new List<CourseAssignment>();
            foreach (string courseStringID in selectedCourses)
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseID==int.Parse(courseStringID));
                //TODO: Figure out: If instructor is not created yet, how can it have an ID??                
                instructor.CourseAssignments.Add(new CourseAssignment { Course = course, Instructor = instructor, CourseID = course.CourseID, InstructorID = instructor.ID});
            }
        }

        if (!String.IsNullOrWhiteSpace(officeLocation))
        {
            instructor.OfficeAssignment = new OfficeAssignment
                { Instructor = instructor, InstructorID = instructor.ID, Location = officeLocation };
        } 
        if (ModelState.IsValid)
        {
            _context.Add(instructor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateAssignedCourseData(instructor);
        return View(instructor);
    }
}