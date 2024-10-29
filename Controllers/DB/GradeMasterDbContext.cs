using GradeMasterAPInew.Controllers.DB.DBModels; // ודא שהנתיב נכון
using Microsoft.EntityFrameworkCore;

namespace GradeMasterAPInew.Controllers.DB
{
    public class GradeMasterDbContext : DbContext
    {
        public GradeMasterDbContext(DbContextOptions<GradeMasterDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // אם לא העברת את הקישור כאן, השאר את זה
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DBGradeMaster;Integrated Security=True;Connect Timeout=30");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Teacher>()
          .HasKey(t => t.TeacherId); // מגדיר את המפתח הראשי


            modelBuilder.Entity<Student>()
                .HasKey(s => s.StudentId); // מגדיר את המפתח הראשי


            modelBuilder.Entity<Course>()
        .HasKey(c => c.CourseId);

            modelBuilder.Entity<Assignment>()
     .HasKey(a => a.AssignmentId); // מגדיר את המפתח הראשי


            modelBuilder.Entity<Attendance>()
     .HasKey(a => a.AttendanceId);

            modelBuilder.Entity<Grade>()
    .HasKey(g => g.GradeId); // מגדיר את המפתח הראשי



            modelBuilder.Entity<Exam>()
                .HasKey(e => e.ExamId); // מגדיר את המפתח הראשי


            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.StudentId, e.CourseId });

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);


            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Exams)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
            .HasOne(c => c.Teacher)
            .WithMany(t => t.Courses)
            .HasForeignKey(c => c.TeacherId);



            modelBuilder.Entity<Course>()
                .HasMany(c => c.FinalGrades)
                .WithOne(g => g.Course)
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Course)
                .WithMany(c => c.FinalGrades)
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

         

            modelBuilder.Entity<Course>()
                .Property(c => c.CourseName)
                .IsRequired(false);

            modelBuilder.Entity<Course>()
                .Property(c => c.CourseDescription)
                .IsRequired(false)
                .HasDefaultValue("Default Description");

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Attendances)
                .WithOne(a => a.Course)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // זהירות מחק לא תומך אם יש רשומות קשורות

          

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.CourseId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
              .HasOne(a => a.Teacher)
              .WithMany()
              .HasForeignKey(a => a.TeacherId)
              .OnDelete(DeleteBehavior.Restrict);
        }

        // Collection Sync with DB
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<ExamSubmission> ExamSubmissions { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
    }
}
