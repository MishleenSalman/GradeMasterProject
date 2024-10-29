namespace GradeMasterAPInew.Controllers.DB.DBModels
{
    public class AssignmentSubmission
    {
        public int AssignmentSubmissionId { get; set; }
        public string? FilePath { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; } 
        public string StudentName { get; set; }

        public DateTime SubmissionDate { get; set; }
        public string Feedback { get; set; }
        public int Grade { get; set; } // שימוש באות גדולה
        public int SubmissionGrade { get; set; } // ציון המטלה שהוקצה


        public virtual Assignment Assignment { get; set; }
        public virtual Student Student { get; set; } 

        public AssignmentSubmission() { }

    }
}
