namespace GradeMasterAPInew.APIModels
{
    public class GradeDTO
    {

        public int GradeId { get; set; }             // מזהה הציון
        public int StudentId { get; set; }           // מזהה הסטודנט
        public int CourseId { get; set; }            // מזהה הקורס
        public int FinalGrade { get; set; }          // הציון הסופי
        public int SubmissionGrade { get; set; }     // ציון המטלה
        public int AttendanceGrade { get; set; }          // ציון נוכחות
        public int ExamGrade { get; set; }          // ציון נוכחות
        public string Feedback { get; set; }
    }
}
