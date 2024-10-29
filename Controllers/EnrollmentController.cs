using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradeMasterAPInew.APIModels;
using GradeMasterAPInew.Controllers.DB.DBModels;
using Microsoft.EntityFrameworkCore;
using GradeMasterAPInew.Controllers.DB;

namespace GradeMasterAPInew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly GradeMasterDbContext _context;

        public EnrollmentController(GradeMasterDbContext context)
        {
            _context = context;
        }

        // GET: api/enrollment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnrollmentDTO>>> Get()
        {
            var enrollmentDTOs = await _context.Enrollments
                .Select(e => new EnrollmentDTO
                {
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollmentDate = e.EnrollmentDate,
                    FinalGrade = e.FinalGrade,
                    // אפשר להוסיף את שמות התלמידים והקורסים אם יש לך את המידע הזה
                }).ToListAsync();

            return Ok(enrollmentDTOs);
        }

        // GET api/enrollment/{studentId}/{courseId}
        [HttpGet("{studentId}/{courseId}")]
        public async Task<ActionResult<EnrollmentDTO>> Get(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
            {
                return NotFound();
            }

            var enrollmentDTO = new EnrollmentDTO
            {
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollmentDate = enrollment.EnrollmentDate,
                FinalGrade = enrollment.FinalGrade
            };

            return Ok(enrollmentDTO);
        }

        // POST api/enrollment
        [HttpPost]
        public async Task<IActionResult> PostEnrollment([FromBody] EnrollmentDTO enrollmentDTO)
        {
            try
            {
                // בדוק אם התלמיד והקורס קיימים
                var studentExists = await _context.Students.AnyAsync(s => s.StudentId == enrollmentDTO.StudentId);
                var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == enrollmentDTO.CourseId);

                if (!studentExists)
                {
                    return BadRequest("התלמיד לא קיים");
                }

                if (!courseExists)
                {
                    return BadRequest("הקורס לא קיים");
                }

                // בדוק אם התלמיד כבר רשום לקורס
                var alreadyEnrolled = await _context.Enrollments.AnyAsync(e => e.StudentId == enrollmentDTO.StudentId && e.CourseId == enrollmentDTO.CourseId);
                if (alreadyEnrolled)
                {
                    return BadRequest("התלמיד כבר רשום לקורס הזה");
                }

                var enrollment = new Enrollment
                {
                    StudentId = enrollmentDTO.StudentId,
                    CourseId = enrollmentDTO.CourseId,
                    EnrollmentDate = enrollmentDTO.EnrollmentDate != DateTime.MinValue ? enrollmentDTO.EnrollmentDate : DateTime.Now,
                    FinalGrade = enrollmentDTO.FinalGrade
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(PostEnrollment), enrollment); // החזר את פרטי ההרשמה
            }
            catch (Exception ex)
            {
                // החזר את השגיאה הפנימית
                return StatusCode(500, $"שגיאה בהוספת ההרשמה: {ex.InnerException?.Message ?? ex.Message}");
            }
        }


        // PUT api/enrollment/{studentId}/{courseId}
        [HttpPut("{studentId}/{courseId}")]
        public async Task<ActionResult> Put(int studentId, int courseId, [FromBody] EnrollmentDTO enrollmentDTO)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
            {
                return NotFound();
            }

            enrollment.FinalGrade = enrollmentDTO.FinalGrade;

            // עדכון במסד הנתונים
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/enrollment/{studentId}/{courseId}
        [HttpDelete("{studentId}/{courseId}")]
        public async Task<ActionResult> Delete(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
            {
                return NotFound();
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
