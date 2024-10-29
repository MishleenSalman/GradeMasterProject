using GradeMasterAPInew.APIModels;
using GradeMasterAPInew.Controllers.DB;
using GradeMasterAPInew.Controllers.DB.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GradesController : ControllerBase
    {
        private readonly GradeMasterDbContext _context;

        public GradesController(GradeMasterDbContext context)
        {
            _context = context;
        }

       

        [HttpGet("attendance/{studentId}")]
        public async Task<ActionResult<AttendanceDTO>> GetAttendanceGrade(int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest("Invalid student ID");
            }

            // ספור את מספר השיעורים הכוללים של הסטודנט
            var totalClasses = await _context.Attendances
                .Where(a => a.StudentId == studentId)
                .CountAsync();

            // ספור את מספר השיעורים שהסטודנט נכח בהם
            var attendedClasses = await _context.Attendances
                .Where(a => a.StudentId == studentId && a.Status == "Present")
                .CountAsync();

            if (totalClasses == 0)
            {
                return NotFound("No attendance records found for this student.");
            }

            // חישוב אחוזי נוכחות
            int attendancePercentage = (attendedClasses * 100) / totalClasses;

            var attendanceDto = new AttendanceDTO
            {
                AttendanceGrade = attendancePercentage // אחוז נוכחות
            };

            return Ok(attendanceDto);
        }


        [HttpGet("submissions/{studentId}")]
        public async Task<ActionResult<AssignmentDTO>> GetSubmissionGrade(int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest("Invalid student ID");
            }

            var submission = await _context.AssignmentSubmissions
                .Where(s => s.StudentId == studentId)
                .Select(s => new AssignmentDTO
                {
                    SubmissionGrade = s.Grade
                })
                .FirstOrDefaultAsync();

            if (submission == null)
            {
                return NotFound();
            }

            return Ok(submission);
        }

        [HttpGet("exams/{studentId}")]
        public async Task<ActionResult<ExamDTO>> GetExamGrade(int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest("Invalid student ID");
            }

            var exam = await _context.ExamSubmissions
                .Where(e => e.StudentId == studentId)
                .Select(e => new ExamDTO
                {
                    ExamGrade = e.Grade
                })
                .FirstOrDefaultAsync();

            if (exam == null)
            {
                return NotFound();
            }

            return Ok(exam);
        }

        [HttpGet("finalGradesForCourse/{courseId}")]
        public IActionResult GetFinalGradesForCourse(int courseId)
        {
            var studentGrades = _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => new
                {
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    AttendanceGrade = _context.Attendances
                        .Where(a => a.StudentId == e.StudentId)
                        .Count(a => a.Status == "Present") * 100 /
                        (_context.Attendances.Count(a => a.StudentId == e.StudentId) == 0 ? 1 : _context.Attendances.Count(a => a.StudentId == e.StudentId)),
                    SubmissionGrade = _context.AssignmentSubmissions
                        .Where(s => s.StudentId == e.StudentId)
                        .Select(s => s.Grade)
                        .FirstOrDefault(),
                    ExamGrade = _context.ExamSubmissions
                        .Where(ex => ex.StudentId == e.StudentId)
                        .Select(ex => ex.Grade)
                        .FirstOrDefault(),
                    FinalGradeData = e.Student.FinalGrades.FirstOrDefault(g => g.CourseId == courseId),
                    FinalGrade = e.FinalGrade, // אם זה השם הנכון של השדה

                    Feedback = e.Student.FinalGrades.FirstOrDefault(g => g.CourseId == courseId) != null ?
                               e.Student.FinalGrades.FirstOrDefault(g => g.CourseId == courseId).Feedback : ""
                }).ToList();

            // בדיקה אם נמצאו ציונים
            if (!studentGrades.Any())
            {
                return NotFound("No students or grades found for this course.");
            }

            // חישוב ממוצע הציונים עבור הקורס
            var averageGrade = studentGrades.Average(g => g.FinalGrade);

            // החזרת נתוני הסטודנטים והממוצע
            return Ok(new { studentGrades, averageGrade });
        }


        [HttpPost("addOrUpdateGrade")]
        public IActionResult AddOrUpdateGrade([FromBody] GradeDTO gradeData)
        {
            if (gradeData == null)
            {
                return BadRequest("Invalid grade data.");
            }

            // בדוק אם הקורס קיים
            var course = _context.Courses.Include(c => c.Teacher).FirstOrDefault(c => c.CourseId == gradeData.CourseId);
            if (course == null)
            {
                return BadRequest("Course does not exist.");
            }

            // אם הקורס קיים, תוודא שהמורה קיים
            if (course.Teacher == null)
            {
                return BadRequest("Course does not have an assigned teacher.");
            }

            var existingGrade = _context.Grades
                .FirstOrDefault(g => g.StudentId == gradeData.StudentId && g.CourseId == gradeData.CourseId);

            if (existingGrade != null)
            {
                // עדכון הציונים אם כבר קיימים
                existingGrade.AttendanceGrade = gradeData.AttendanceGrade;
                existingGrade.SubmissionGrade = gradeData.SubmissionGrade;
                existingGrade.ExamGrade = gradeData.ExamGrade;
                existingGrade.FinalGrade = gradeData.FinalGrade;
                existingGrade.Feedback = gradeData.Feedback;

                _context.Grades.Update(existingGrade);
            }
            else
            {
                // יצירת רישום חדש אם לא קיים
                var newGrade = new Grade
                {
                    StudentId = gradeData.StudentId,
                    CourseId = gradeData.CourseId,
                    AttendanceGrade = gradeData.AttendanceGrade,
                    SubmissionGrade = gradeData.SubmissionGrade,
                    ExamGrade = gradeData.ExamGrade,
                    FinalGrade = gradeData.FinalGrade,
                    Feedback = gradeData.Feedback
                };

                _context.Grades.Add(newGrade);
            }

            _context.SaveChanges(); // שמירה של השינויים ב-Database

            return Ok("Grade added/updated successfully.");
        }

        
        // פונקציה לחישוב הציון הסופי (שכחתי מהחישוב המדויק שלך, תעדכן בהתאם)
        private double CalculateFinalGrade(int attendanceGrade, int submissionGrade, int examGrade)
        {
            // לדוגמה, אם תכננת את המשקלות כך: 
            double finalGrade = (attendanceGrade * 0.2) + (submissionGrade * 0.3) + (examGrade * 0.5);
            return finalGrade;
        }






    }
}
