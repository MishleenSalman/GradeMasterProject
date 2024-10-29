namespace GradeMasterAPInew.APIModels
{
    public class TeacherDTO
    {
        public int TeacherId { get; set; }              // מזהה המורה
        public string? Email { get; set; }
        public string Password { get; set; }
        public string? FirstName { get; set; }           // שם פרטי
        public string? LastName { get; set; }            // שם משפחה
        public string? PhoneNumber { get; set; }
    }
}
