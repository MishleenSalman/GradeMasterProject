using System.Diagnostics;

namespace GradeMasterAPInew.Controllers.DB.DBModels
{
    public class Course
    {

        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }
        public int TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; } 

        #region -----NAVIGATION PROPERTIES-----
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Grade> FinalGrades { get; set; } = new List<Grade>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        #endregion

        public Course() { }
    }
}
