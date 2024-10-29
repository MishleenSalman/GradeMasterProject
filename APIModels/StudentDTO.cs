namespace GradeMasterAPInew.APIModels
{
    public class StudentDTO
    {
        public int StudentId { get; set; }             // מזהה הסטודנט
        public string? FirstName { get; set; }          // שם פרטי
        public string? LastName { get; set; }           // שם משפחה
        public DateTime DateBirth { get; set; }        // תאריך לידה
        public string? Gender { get; set; }             // מגדר
        public string? PhoneNumber { get; set; }        // מספר טלפון
        public string? Address { get; set; }            // כתובת
        public string? Email { get; set; }              // כתובת אימייל
        public DateTime EnrollmentDate { get; set; }
    }
}
