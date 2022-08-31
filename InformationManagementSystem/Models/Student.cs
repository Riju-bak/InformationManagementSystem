using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InformationManagementSystem.Models;

public class Student
{
    public int ID { get; set; }     //PK
    
    [Required]
    [StringLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    
    //The Column attribute specifies that when the database is created, the column of the Student table that maps to the FirstMidName property will be named FirstName
    //In other words, when your code refers to Student.FirstMidName, the data will come from or be updated in the FirstName column of the Student table. 
    [StringLength(50)]
    [Column("FirstName")] 
    [Display(Name = "First Name")]

    public string FirstMidName { get; set; }
    
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Display(Name = "Enrollment Date")]
    public DateTime EnrollmentDate { get; set; }

    [Display(Name = "Full Name")]
    public string FullName
    {
        get
        {
            return LastName + ", " + FirstMidName;
        }
    }

    public ICollection<Enrollment>? Enrollments { get; set; }  //Called a collection Navigaton property
}