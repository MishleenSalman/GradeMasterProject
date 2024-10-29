namespace GradeMasterAPInew.Controllers.DB.DBModels
{
    public class Teacher
    {

        public int TeacherId { get; set; }

        public string? Email { get; set; }

        public string Password { get; set; } 

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public ICollection<Course> Courses { get; set; }

        
        public Teacher() { }
    }
}
