using System.ComponentModel.DataAnnotations;

namespace InformationManagementSystem.Models;

public class OfficeAssignment
{
    // There's a one to zero-or-one relationship between the Instructor and the OfficeAssignment entities. 
    // An office assignment only exists in relation to the instructor it's assigned to, and therefore its primary key is also its foreign key to the Instructor entity.
    //But the Entity Framework can't automatically recognize InstructorID as the primary key of this entity because its name doesn't follow the ID or classnameID naming convention.
    // Therefore, the Key attribute is used to identify it as the key
    [Key]
    public int InstructorID { get; set; }

    [StringLength(50)]
    public string Location { get; set; }

    //nav prop
    public Instructor Instructor { get; set; }
}