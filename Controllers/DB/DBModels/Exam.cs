namespace GradeMasterAPInew.Controllers.DB.DBModels
{
    public class Exam
    {

        public int ExamId { get; set; }

        public string ExamName { get; set; }

        public DateTime ExamDate { get; set; }
        public string RoomNumber { get; set; }
        public int DurationMinutes { get; set; }

        public int CourseId { get; set; }

        public virtual Course Course { get; set; }
        public int StudentId { get; internal set; }
        public int ExamGrade { get; set; }

        public Exam() { }
    }
}
