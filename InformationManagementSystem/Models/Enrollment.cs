using System.ComponentModel.DataAnnotations;

namespace InformationManagementSystem.Models;

public enum Grade
{
    A, B, C, D, F
}

public class Enrollment
{
    public int EnrollmentID { get; set; }   //PK
    public int CourseID { get; set; }       //FK
    public int StudentID { get; set; }      //FK
    
    [DisplayFormat(NullDisplayText = "No grade")]
    public Grade? Grade { get; set; }
    
    //These are called reference navigaton properties
    public Course Course { get; set; } 
    public Student Student { get; set; }
    
}