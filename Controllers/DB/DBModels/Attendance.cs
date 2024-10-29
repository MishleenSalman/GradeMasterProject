namespace GradeMasterAPInew.Controllers.DB.DBModels
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public string RoomNumber { get; set; }
        public DateTime Start { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int TeacherId { get; set; }
        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; } 
        public virtual Teacher Teacher { get; set; } 

        public Attendance() { }

    }
}
