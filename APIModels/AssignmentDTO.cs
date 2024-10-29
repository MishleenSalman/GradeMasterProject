namespace GradeMasterAPInew.APIModels
{
    public class AssignmentDTO
    {
        public int AssignmentId { get; set; }          // מזהה המטלה
        public string? Title { get; set; }               // כותרת המטלה
        public string? Description { get; set; }        // תיאור המטלה
        public DateTime DueDate { get; set; }           // תאריך הגשה
        public int SubmissionGrade { get; set; }

        public int CourseId { get; set; }
    }
}
