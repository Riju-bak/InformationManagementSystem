using System.ComponentModel.DataAnnotations;

namespace InformationManagementSystem.Models.SchoolViewModels;

public class EnrollmentDateGroup
{
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Display(Name = "Enrollment Date")]
    public DateTime EnrollmentDate { get; set; }
    
    public int StudentCount { get; set; }
}