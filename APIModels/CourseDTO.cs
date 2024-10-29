namespace GradeMasterAPInew.APIModels
{
    public class CourseDTO
    {
        public int? CourseId { get; set; }           // ID של הקורס
        public string? CourseName { get; set; }      // שם הקורס
        public string? CourseDescription { get; set; } // תיאור הקורס
        public int TeacherId { get; set; }          // ID של המורה

    }
}
