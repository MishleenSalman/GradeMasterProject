namespace GradeMasterAPInew.Controllers.DB.DBModels
{
    public class ExamSubmission
    {

        public int ExamSubmissionId { get; set; }

        public string FilePath { get; set; }

        public int ExamId { get; set; }
        public int StudentId { get; set; }

        public DateTime SubmissionDate { get; set; }
        public int Grade { get; set; }
        public string Feedback { get; set; }


        public virtual Exam Exam { get; set; }
        public virtual Student Student { get; set; } 
        public ExamSubmission() { }
    }
}
