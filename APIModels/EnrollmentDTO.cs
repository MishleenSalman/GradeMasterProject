namespace GradeMasterAPInew.APIModels
{
    public class EnrollmentDTO
    {
        public int StudentId { get; set; }            // מזהה התלמיד
        public int CourseId { get; set; }             // מזהה הקורס
        public DateTime EnrollmentDate { get; set; }   // תאריך ההרשמה
        public int? FinalGrade { get; set; }          // אפשרות לציון סופי ריק

        // ניתן להוסיף מידע נוסף אם יש צורך
        public string StudentName { get; set; }      // שם התלמיד (אם יש צורך להציג את השם)
        public string CourseName { get; set; }
    }
}
