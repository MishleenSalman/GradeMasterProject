using GradeMasterAPInew.Controllers.DB.DBModels;

namespace GradeMasterAPInew.APIModels
{
    public class ExamDTO
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public DateTime ExamDate { get; set; }
        public string RoomNumber { get; set; }
        public int DurationMinutes { get; set; }
        public int CourseId { get; set; } // ID של הקורס
        public int ExamGrade { get; set; }


    }
}
