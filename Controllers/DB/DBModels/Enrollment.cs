namespace GradeMasterAPInew.Controllers.DB.DBModels
{
    public class Enrollment
    {
        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.Now; // אתחול לערך ברירת מחדל
        public int? FinalGrade { get; set; } // אפשרות לציון סופי ריק

        public virtual Student Student { get; set; } 
        public virtual Course Course { get; set; } 

        public Enrollment() { }
    }
}
