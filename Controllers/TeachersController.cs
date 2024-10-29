using GradeMasterAPInew.APIModels;
using GradeMasterAPInew.Controllers.DB;
using GradeMasterAPInew.Controllers.DB.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GradeMasterAPInew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly GradeMasterDbContext _context;

        public TeachersController(GradeMasterDbContext context)
        {
            _context = context;
        }

        // GET: api/<TeachersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherDTO>>> GetTeachers()
        {
            var teachers = await _context.Teachers
                .Select(t => new TeacherDTO
                {
                    TeacherId = t.TeacherId,
                    Email = t.Email,
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    PhoneNumber = t.PhoneNumber
                }).ToListAsync();

            return Ok(teachers);
        }

        // GET api/<TeachersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDTO>> GetTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            var teacherDTO = new TeacherDTO
            {
                TeacherId = teacher.TeacherId,
                Email = teacher.Email,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                PhoneNumber = teacher.PhoneNumber
            };

            return Ok(teacherDTO);
        }

        [HttpGet("{teacherId}/courses")]
        public IActionResult GetCoursesByTeacher(int teacherId)
        {
            var courses = _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .Select(c => new
                {
                    c.CourseId,
                    c.CourseName
                })
                .ToList();

            if (courses == null || !courses.Any())
            {
                return NotFound("No courses found for this teacher.");
            }

            return Ok(courses);
        }

        // POST api/<TeachersController>
        [HttpPost]
   
        public async Task<ActionResult<TeacherDTO>> PostTeacher(TeacherDTO teacherDto)
        {
            if (teacherDto == null)
            {
                return BadRequest("יש לספק נתוני מורה.");
            }

            // בדיקה אם המורה כבר קיים לפי אימייל
            var existingTeacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Email == teacherDto.Email);
            if (existingTeacher != null)
            {
                return BadRequest("מורה עם האימייל שסופק כבר קיים במערכת.");
            }

            // יצירת אובייקט מורה חדש
            var teacher = new Teacher
            {
                FirstName = teacherDto.FirstName,
                LastName = teacherDto.LastName,
                Email = teacherDto.Email,
                PhoneNumber = teacherDto.PhoneNumber,
                Password = teacherDto.Password // אם צריך להצפין סיסמה, אפשר לעשות זאת כאן
            };

            // הוספת המורה למסד הנתונים ושמירה
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            // החזרת התוצאה עם המורה שנוסף
            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.TeacherId }, new TeacherDTO
            {
                TeacherId = teacher.TeacherId,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                PhoneNumber = teacher.PhoneNumber
            });
        }

        // POST api/teacher/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginRequest)
        {
            Console.WriteLine($"Received Email: {loginRequest.Email}");
            Console.WriteLine($"Received Password: {loginRequest.Password}");

            var teacher = await _context.Teachers.SingleOrDefaultAsync(t => t.Email == loginRequest.Email);

            if (teacher == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            if (loginRequest.Password != teacher.Password)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // החזר את ה-ID של המורה אם ההתחברות הצליחה
            return Ok(new { Id = teacher.TeacherId, message = "Login successful" });
        }

        [HttpPost("submission/{submissionId}/grade")]
        public IActionResult UpdateGrade(int submissionId, [FromBody] GradeDTO model)
        {
            var submission = _context.AssignmentSubmissions.FirstOrDefault(s => s.AssignmentSubmissionId == submissionId);
            if (submission == null) return NotFound();

            submission.Grade = model.SubmissionGrade;
            submission.Feedback = model.Feedback;

            _context.SaveChanges();
            return Ok();
        }

        // PUT api/<TeachersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(int id, TeacherDTO teacherDTO)
        {
            if (id != teacherDTO.TeacherId)
            {
                return BadRequest();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            teacher.Email = teacherDTO.Email;
            teacher.FirstName = teacherDTO.FirstName;
            teacher.LastName = teacherDTO.LastName;
            teacher.PhoneNumber = teacherDTO.PhoneNumber;

            _context.Entry(teacher).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<TeachersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("deleteall")]
        public async Task<IActionResult> DeleteAllTeachers()
        {
            var teachers = await _context.Teachers.ToListAsync();

            if (teachers.Count == 0)
            {
                return NotFound("אין מורים למחוק.");
            }

            _context.Teachers.RemoveRange(teachers);
            await _context.SaveChangesAsync();

            return Ok("כל המורים נמחקו בהצלחה.");
        }

        // Helper method to hash passwords
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }




       


    }
}

