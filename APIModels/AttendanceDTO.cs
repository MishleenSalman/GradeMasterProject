namespace GradeMasterAPInew.APIModels
{
    public class AttendanceDTO
    {
        public int AttendanceId { get; set; }
        public string? RoomNumber { get; set; }
        public DateTime Start { get; set; }
        public int DurationMinutes { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public int StudentId { get; set; } // במקרים מסוימים, אפשר לשקול להוסיף Student Name
        public int CourseId { get; set; }  // במקרים מסוימים, אפשר לשקול להוסיף Course Name
        public int TeacherId { get; set; }  // במקרים מסוימים, אפשר לשקול להוסיף Teacher Name
        public int AttendanceGrade { get; set; }

    }
}
