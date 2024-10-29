namespace GradeMasterAPInew.Controllers.DB.DBModels
{
    public class Student
    {


        public int StudentId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateBirth { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        public DateTime EnrollmentDate { get; set; }

        // Connections (one to one and one to many)
        public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
        public ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = new HashSet<AssignmentSubmission>();
        public ICollection<ExamSubmission> ExamSubmissions { get; set; } = new HashSet<ExamSubmission>();
        public ICollection<Attendance> Attendances { get; set; } = new HashSet<Attendance>();
        public ICollection<Grade> FinalGrades { get; set; } = new HashSet<Grade>();

        public Student() { }
    }
}
