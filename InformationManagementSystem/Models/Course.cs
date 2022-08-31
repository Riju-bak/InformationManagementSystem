using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InformationManagementSystem.Models;

public class Course
{
    //This data annotation allows entering PK for the course without having the database generate it.
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Number")]
    public int CourseID { get; set; }

    [StringLength(50, MinimumLength = 3)]
    public string Title { get; set; }

    [Range(0,5)]
    public int Credits { get; set; }

    public int DepartmentID { get; set; }   //FK

    //nav props
    public Department Department { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
    public ICollection<CourseAssignment> CourseAssignments { get; set; }
}