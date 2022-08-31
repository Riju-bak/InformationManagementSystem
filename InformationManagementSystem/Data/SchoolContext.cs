using InformationManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InformationManagementSystem.Data;

public class SchoolContext : DbContext
{
    public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }                      //Create Table
    public DbSet<Enrollment> Enrollments { get; set; }              //Create Table
    public DbSet<Student> Students { get; set; }                    //Create Table

    public DbSet<Instructor> Instructors { get; set; }
    
    public DbSet<Department> Departments { get; set; }
    
    public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
   
    public DbSet<CourseAssignment> CourseAssignments { get; set; }

    //Developers disagree about whether table names should be pluralized or not.
    //For these tutorials, the default behavior is overridden by specifying singular table
    //names in the DbContext
    //NOTE: This can be also used to create tables with names of your choice
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>().ToTable("Course");
        modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
        modelBuilder.Entity<Student>().ToTable("Student");
        modelBuilder.Entity<Instructor>().ToTable("Instructor");
        modelBuilder.Entity<Department>().ToTable("Department");
        modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");
        modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment");
        
        //Set the primary key for CourseAssignment to be a composite key (CourseID, InstructorID)
        modelBuilder.Entity<CourseAssignment>()
            .HasKey(c => new { c.CourseID, c.InstructorID });        
    }
}