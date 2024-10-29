namespace GradeMasterAPInew.Controllers.DB.DBModels
{
    public class Grade
    {
        public int GradeId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int FinalGrade { get; set; }
        public int SubmissionGrade { get; set; }
        public string Feedback { get; set; }
        public int ExamGrade { get; set; }        

        public int AttendanceGrade { get; set; }
        public virtual Student Student { get; set; } = new Student();
        public virtual Course Course { get; set; } = new Course();

        public Grade() { }
    }
}
