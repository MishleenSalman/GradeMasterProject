namespace GradeMasterAPInew.APIModels
{
    public class ExamSubmissionDTO
    {
        public int ExamSubmissionId { get; set; }
        public string FilePath { get; set; }
        public int ExamId { get; set; } // ID של המבחן
        public int StudentId { get; set; } // ID של התלמיד
        public string Feedback { get; set; }

        public string StudentName { get; set; }

        public DateTime SubmissionDate { get; set; }
        public int Grade { get; set; } // הציון
    }
}
