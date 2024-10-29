using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradeMasterAPInew.Controllers.DB.DBModels;
using GradeMasterAPInew.APIModels;
using GradeMasterAPInew.Controllers.DB;

namespace GradeMasterAPInew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly GradeMasterDbContext _context;

        public ExamController(GradeMasterDbContext context)
        {
            _context = context;
        }

        // GET: api/Exam
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamDTO>>> GetAllExams()
        {
            var exams = await _context.Exams
                .Select(e => new ExamDTO
                {
                    ExamId = e.ExamId,
                    ExamName = e.ExamName,
                    ExamDate = e.ExamDate,
                    RoomNumber = e.RoomNumber,
                    DurationMinutes = e.DurationMinutes,
                    CourseId = e.CourseId
                })
                .ToListAsync();

            return exams;
        }

        // GET: api/Exam/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamDTO>> GetExam(int id)
        {
            var exam = await _context.Exams
                .Where(e => e.ExamId == id)
                .Select(e => new ExamDTO
                {
                    ExamId = e.ExamId,
                    ExamName = e.ExamName,
                    ExamDate = e.ExamDate,
                    RoomNumber = e.RoomNumber,
                    DurationMinutes = e.DurationMinutes,
                    CourseId = e.CourseId
                })
                .FirstOrDefaultAsync();

            if (exam == null)
            {
                return NotFound();
            }

            return exam;
        }
        [HttpGet("GetExamsByCourse/{courseId}")]
        public IActionResult GetExamsByCourse(int courseId)
        {
            var exams = _context.Exams
                .Where(e => e.CourseId == courseId)
                .Select(e => new ExamDTO
                {
                    ExamId = e.ExamId,
                    ExamName = e.ExamName,
                    ExamDate = e.ExamDate,
                    RoomNumber = e.RoomNumber,
                    DurationMinutes = e.DurationMinutes,
                    CourseId = e.CourseId
                })
                .ToList();

            if (exams == null || exams.Count == 0)
            {
                return NotFound("No exams found for this course.");
            }

            return Ok(exams);
        }

        [HttpPost]
        public async Task<ActionResult<ExamDTO>> CreateExam(ExamDTO examDto)
        {
              // Validate the input
                if (examDto.CourseId <= 0)
                {
                    return BadRequest("Invalid Course ID.");
                }

                // Check if the course exists
                var course = await _context.Courses.FindAsync(examDto.CourseId);
                if (course == null)
                {
                    return NotFound("Course not found.");
                }

                var exam = new Exam
                {
                    ExamName = examDto.ExamName,
                    ExamDate = examDto.ExamDate,
                    RoomNumber = examDto.RoomNumber,
                    DurationMinutes = examDto.DurationMinutes,
                    CourseId = examDto.CourseId // Set the foreign key directly
                };

                _context.Exams.Add(exam);
                await _context.SaveChangesAsync();

            return Ok(new { message = "Exam created successfully", ExamId = exam.ExamId });
        }


       

    }
}
