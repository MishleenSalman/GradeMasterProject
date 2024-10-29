using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradeMasterAPInew.Controllers.DB.DBModels;
using GradeMasterAPInew.APIModels;
using GradeMasterAPInew.Controllers.DB;

namespace GradeMasterAPInew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentSubmissionController : ControllerBase
    {
        private readonly GradeMasterDbContext _context;

        public AssignmentSubmissionController(GradeMasterDbContext context)
        {
            _context = context;
        }

        // GET: api/AssignmentSubmission
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssignmentSubmissionDTO>>> GetAllSubmissions()
        {
            var submissions = await _context.AssignmentSubmissions
                .Select(s => new AssignmentSubmissionDTO
                {
                    AssignmentSubmissionId = s.AssignmentSubmissionId,
                    AssignmentId = s.AssignmentId,
                    StudentId = s.StudentId,
                    SubmissionDate = s.SubmissionDate,
                    Feedback = s.Feedback,
                    SubmissionGrade = s.Grade // עדכון לשדה SubmissionGrade
                })
                .ToListAsync();

            return submissions;
        }

        // GET: api/AssignmentSubmission/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AssignmentSubmissionDTO>> GetSubmission(int id)
        {
            var submission = await _context.AssignmentSubmissions
                .Where(s => s.AssignmentSubmissionId == id)
                .Select(s => new AssignmentSubmissionDTO
                {
                    AssignmentSubmissionId = s.AssignmentSubmissionId,
                    AssignmentId = s.AssignmentId,
                    StudentId = s.StudentId,
                    SubmissionDate = s.SubmissionDate,
                    Feedback = s.Feedback,
                    SubmissionGrade = s.Grade // עדכון לשדה SubmissionGrade
                })
                .FirstOrDefaultAsync();

            if (submission == null)
            {
                return NotFound();
            }

            return submission;
        }

        // GET: api/ExamSubmission/student/{studentId}/exam/{examId}
        [HttpGet("student/{studentId}/assignment/{assignmentId}")]
        public async Task<ActionResult<int>> GetAssiSubmissionId(int studentId, int assignmentId)
        {
            var submission = await _context.AssignmentSubmissions
                .Where(s => s.StudentId == studentId && s.AssignmentId == assignmentId)
                .Select(s => s.AssignmentSubmissionId)
                .FirstOrDefaultAsync();

            if (submission == 0)
            {
                return NotFound("ExamSubmission not found.");
            }

            return Ok(submission);
        }

        [HttpGet("submissions/course/{courseId}/assignment/{assignmentTitle}")]
        public async Task<ActionResult<IEnumerable<AssignmentSubmissionDTO>>> GetSubmissionsByCourseAndAssignmentTitle(int courseId, string assignmentTitle)
        {
            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.CourseId == courseId && a.Title == assignmentTitle);

            if (assignment == null)
            {
                return NotFound("Assignment not found.");
            }

            var submissions = await _context.AssignmentSubmissions
                .Where(s => s.AssignmentId == assignment.AssignmentId)
                .Select(s => new
                {
                    s.AssignmentSubmissionId,
                    s.AssignmentId,
                    s.StudentId,
                    s.SubmissionDate,
                    s.Feedback,
                    s.Grade
                })
                .ToListAsync();

            if (submissions == null || submissions.Count == 0)
            {
                return NotFound("No submissions found for this assignment.");
            }

            var studentIds = submissions.Select(s => s.StudentId).Distinct().ToList();
            var students = await _context.Students
                .Where(st => studentIds.Contains(st.StudentId))
                .Select(st => new
                {
                    st.StudentId,
                    st.FirstName
                })
                .ToListAsync();

            var result = submissions.Select(s => new AssignmentSubmissionDTO
            {
                AssignmentSubmissionId = s.AssignmentSubmissionId,
                AssignmentId = s.AssignmentId,
                StudentId = s.StudentId,
                SubmissionDate = s.SubmissionDate,
                Feedback = s.Feedback,
                SubmissionGrade = s.Grade,
                StudentName = students.FirstOrDefault(st => st.StudentId == s.StudentId)?.FirstName
            }).ToList();

            return result;
        }

        [HttpPost]
        public async Task<ActionResult<AssignmentSubmissionDTO>> CreateSubmission(AssignmentSubmissionDTO submissionDTO)
        {
            if (submissionDTO == null)
            {
                return BadRequest();
            }

            var submission = new AssignmentSubmission
            {
                AssignmentId = submissionDTO.AssignmentId,
                StudentId = submissionDTO.StudentId,
                StudentName = submissionDTO.StudentName,
                SubmissionDate = submissionDTO.SubmissionDate,
                Feedback = submissionDTO.Feedback,
                Grade = submissionDTO.SubmissionGrade, // עדכון לשדה Grade
                FilePath = submissionDTO.FilePath ?? string.Empty // הוסף את FilePath אם קיים או הגדר ערך ריק
            };

            _context.AssignmentSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            submissionDTO.AssignmentSubmissionId = submission.AssignmentSubmissionId;

            return CreatedAtAction(nameof(GetSubmission), new { id = submissionDTO.AssignmentSubmissionId }, submissionDTO);
        }

        

        [HttpPost("course/{courseId}/assignment/{assignmentId}/student/{studentId}/grade")]
        public async Task<IActionResult> UpdateOrAddGrade(int courseId, int assignmentId, int studentId, [FromBody] AssignmentSubmissionDTO gradeUpdate)
        {
            if (gradeUpdate == null)
            {
                return BadRequest("Invalid grade update data.");
            }

            var submission = await _context.AssignmentSubmissions
                .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);

            if (submission != null)
            {
                submission.Grade = gradeUpdate.SubmissionGrade; // עדכון לשדה Grade
                submission.Feedback = gradeUpdate.Feedback;
                submission.FilePath = gradeUpdate.FilePath ?? submission.FilePath; // עדכון FilePath אם קיים

                await _context.SaveChangesAsync();
                return Ok(new { message = "Grade updated successfully." });
            }
            else
            {
                var newSubmission = new AssignmentSubmission
                {
                    AssignmentId = assignmentId,
                    StudentId = studentId,
                    StudentName = gradeUpdate.StudentName,
                    SubmissionGrade = gradeUpdate.SubmissionGrade, // עדכון לשדה SubmissionGrade
                    Feedback = gradeUpdate.Feedback,
                    SubmissionDate = DateTime.UtcNow,
                    FilePath = gradeUpdate.FilePath ?? string.Empty // הוסף FilePath אם קיים או ערך ריק
                };

                await _context.AssignmentSubmissions.AddAsync(newSubmission);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(UpdateOrAddGrade), new { courseId, assignmentId, studentId }, newSubmission);
            }
        }
    }
}
