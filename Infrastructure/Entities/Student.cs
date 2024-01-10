using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class Student
{
    [Key]
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
    public DateTime CreatedDate { get; set;}
    public DateTime UpdateDate { get; set;}
    
}
