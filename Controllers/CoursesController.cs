using GradeMasterAPInew.APIModels;
using GradeMasterAPInew.Controllers.DB;
using GradeMasterAPInew.Controllers.DB.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GradeMasterAPInew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly GradeMasterDbContext _context;

        public CoursesController(GradeMasterDbContext context)
        {
            _context = context;
        }

        // GET: api/<CoursesController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetCourses()
        {
            var courses = await _context.Courses
                .Select(c => new CourseDTO
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    CourseDescription = c.CourseDescription,
                    TeacherId = c.TeacherId
                }).ToListAsync();

            return Ok(courses);
        }

        // GET api/<CoursesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDTO>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            var courseDto = new CourseDTO
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                CourseDescription = course.CourseDescription,
                TeacherId = course.TeacherId
            };

            return Ok(courseDto);
        }


        // GET: api/<CoursesController>/byTeacher/{teacherId}
        [HttpGet("byTeacher/{teacherId}")]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetCoursesByTeacher(int teacherId)
        {
            var courses = await _context.Courses
                .Where(c => c.TeacherId == teacherId)  // מסנן לפי מזהה המורה
                .Select(c => new CourseDTO
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    CourseDescription = c.CourseDescription,
                    TeacherId = c.TeacherId
                }).ToListAsync();

            if (courses.Count == 0)
            {
                return NotFound("אין קורסים למורה זה.");
            }

            return Ok(courses);
        }

        [HttpGet("GetStudentsByCourse/{courseId}")]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudentsByCourse(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            var studentDTOs = course.Enrollments.Select(e => new StudentDTO
            {
                StudentId = e.Student.StudentId,
                FirstName = e.Student.FirstName,
                LastName = e.Student.LastName
            }).ToList();

            return Ok(studentDTOs);
        }

        [HttpGet("course/{courseId}/assignments")]
        public IActionResult GetCourseAssignments(int courseId)
        {
            var assignments = _context.Assignments
                .Where(a => a.CourseId == courseId)
                .ToList();
            return Ok(assignments);
        }

        [HttpGet("course/{courseId}/submissions")]
        public IActionResult GetCourseSubmissions(int courseId)
        {
            var submissions = _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => new
                {
                    StudentId = e.StudentId,
                    StudentName = e.Student.FirstName,
                    Submissions = _context.AssignmentSubmissions
                        .Where(s => s.StudentId == e.StudentId && s.Assignment.CourseId == courseId)
                        .ToList()
                })
                .ToList();
            return Ok(submissions);
        }



        // POST api/<CoursesController>
        [HttpPost]
        public async Task<ActionResult<CourseDTO>> PostCourse(CourseDTO courseDto)
        {
            if (courseDto == null)
            {
                return BadRequest("חובה להזין נתוני קורס.");
            }

            var teacher = await _context.Teachers.FindAsync(courseDto.TeacherId);
            if (teacher == null)
            {
                return BadRequest("מורה עם ה-ID שסופק לא קיים.");
            }

            var course = new Course
            {
                CourseName = courseDto.CourseName,
                CourseDescription = courseDto.CourseDescription,
                TeacherId = courseDto.TeacherId,
                Teacher = teacher  // שייך את המורה הקיים לקורס
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, new CourseDTO
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                CourseDescription = course.CourseDescription,
                TeacherId = course.TeacherId
            });
        }

        // PUT api/<CoursesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, CourseDTO courseDto)
        {
            if (id != courseDto.CourseId)
            {
                return BadRequest();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            course.CourseName = courseDto.CourseName;
            course.CourseDescription = courseDto.CourseDescription;
            course.TeacherId = courseDto.TeacherId;

            _context.Entry(course).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<CoursesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("deleteall")]
        public async Task<IActionResult> DeleteAllCourses()
        {
            var courses = await _context.Courses.ToListAsync();

            if (courses.Count == 0)
            {
                return NotFound("אין קורסים למחוק.");
            }

            _context.Courses.RemoveRange(courses);
            await _context.SaveChangesAsync();

            return Ok("כל הקורסים נמחקו בהצלחה.");
        }



    }
}
