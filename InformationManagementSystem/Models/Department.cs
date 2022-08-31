using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InformationManagementSystem.Models;

public class Department
{
    public int DepartmentID { get; set; }

    [StringLength(50, MinimumLength = 3)]
    public string Name { get; set; }

    //Earlier we used the Column attribute to change column name mapping. 
    // n the code for the Department entity, the Column attribute is being used to change SQL data type mapping
    // so that the column will be defined using the SQL Server money type in the database
    [DataType(DataType.Currency)]
    [Column(TypeName = "money")]
    public decimal Budget { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime StartDate { get; set; }

    public int? InstructorID { get; set; }

    public Instructor Administrator { get; set; }
    public ICollection<Course> Courses { get; set; }
}