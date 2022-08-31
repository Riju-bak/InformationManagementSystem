namespace InformationManagementSystem.Models;

public class CourseAssignment
{
    //Since the foreign keys are not nullable and together uniquely identify each row of the table,
    //there's no need for a separate primary key.
    
    // The InstructorID and CourseID properties should function as a composite primary key.
    
    //We achieve this in OnModelCreating() in SchoolContext.cs
    public int CourseID { get; set; }
    public int InstructorID { get; set; }
    
    //nav props
    public Course Course { get; set; }
    public Instructor Instructor { get; set; }
}