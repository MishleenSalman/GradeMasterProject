namespace GradeMasterAPInew.APIModels
{
    public class AssignmentSubmissionDTO
    {
        public int AssignmentSubmissionId { get; set; }
        public int AssignmentId { get; set; }
        public string? FilePath { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string? Feedback { get; set; }
        public int SubmissionGrade { get; set; }
    }
}
