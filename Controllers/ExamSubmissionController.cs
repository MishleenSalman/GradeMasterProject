using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradeMasterAPInew.Controllers.DB.DBModels;
using GradeMasterAPInew.APIModels;
using GradeMasterAPInew.Controllers.DB;

namespace GradeMasterAPInew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamSubmissionController : ControllerBase
    {
        private readonly GradeMasterDbContext _context;

        public ExamSubmissionController(GradeMasterDbContext context)
        {
            _context = context;
        }

        // GET: api/ExamSubmission
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamSubmissionDTO>>> GetAllSubmissions()
        {
            var submissions = await _context.ExamSubmissions
                .Select(s => new ExamSubmissionDTO
                {
                    ExamSubmissionId = s.ExamSubmissionId,
                    FilePath = s.FilePath,
                    ExamId = s.ExamId,
                    StudentId = s.StudentId,
                    SubmissionDate = s.SubmissionDate,
                    Grade = s.Grade
                })
                .ToListAsync();

            return submissions;
        }

        // GET: api/ExamSubmission/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamSubmissionDTO>> GetSubmission(int id)
        {
            var submission = await _context.ExamSubmissions
                .Where(s => s.ExamSubmissionId == id)
                .Select(s => new ExamSubmissionDTO
                {
                    ExamSubmissionId = s.ExamSubmissionId,
                    FilePath = s.FilePath,
                    ExamId = s.ExamId,
                    StudentId = s.StudentId,
                    SubmissionDate = s.SubmissionDate,
                    Grade = s.Grade
                })
                .FirstOrDefaultAsync();

            if (submission == null)
            {
                return NotFound();
            }

            return submission;
        }


        // GET: api/ExamSubmission/student/{studentId}/exam/{examId}
        [HttpGet("student/{studentId}/exam/{examId}")]
        public async Task<ActionResult<int>> GetExamSubmissionId(int studentId, int examId)
        {
            // חפש את ההגשה לפי הסטודנט והבחינה
            var submission = await _context.ExamSubmissions
                .Where(s => s.StudentId == studentId && s.ExamId == examId)
                .Select(s => s.ExamSubmissionId) // קח רק את ה-ExamSubmissionId
                .FirstOrDefaultAsync();

            if (submission == 0)
            {
                return NotFound("ExamSubmission not found.");
            }

            return Ok(submission); // החזר את ה-ExamSubmissionId
        }


        [HttpGet("submissions/course/{courseId}/exam/{examId}")]
        public async Task<ActionResult<IEnumerable<ExamSubmissionDTO>>> GetExamSubmissions(int courseId, int examId)
        {
            var exam = await _context.Exams
                .FirstOrDefaultAsync(a => a.CourseId == courseId && a.ExamId == examId);

            if (exam == null)
            {
                return NotFound("exam not found.");
            }

            var submissions = await _context.ExamSubmissions
                .Where(s => s.ExamId == examId)
                .Select(s => new ExamSubmissionDTO
                {
                    StudentId = s.StudentId,
                    FilePath = s.FilePath,
                    StudentName = s.Student.FirstName, // כולל את שם הסטודנט כאן
                    Grade = s.Grade,
                    Feedback = s.Feedback
                })
                .ToListAsync();

            return Ok(submissions);
        }
        [HttpPost]
        public async Task<ActionResult<ExamSubmissionDTO>> CreateExamSubmission(ExamSubmissionDTO submissionDTO)
        {
            if (submissionDTO == null)
            {
                return BadRequest();
            }

            var submission = new ExamSubmission
            {
                FilePath = submissionDTO.FilePath,
                ExamId = submissionDTO.ExamId,
                StudentId = submissionDTO.StudentId,
                SubmissionDate = submissionDTO.SubmissionDate,
                Feedback = submissionDTO.Feedback,
                Grade = submissionDTO.Grade
            };

            _context.ExamSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            submissionDTO.ExamSubmissionId = submission.ExamSubmissionId;

            return CreatedAtAction(nameof(GetSubmission), new { id = submissionDTO.ExamSubmissionId }, submissionDTO);
        }



        [HttpPut("course/{courseId}/exam/{examId}/student/{studentId}/grade")]
        public async Task<IActionResult> UpdateOrAddExamGrade(int courseId, int examId, int studentId, [FromBody] ExamSubmissionDTO gradeUpdate)
        {
            // בדוק אם הנתונים שהתקבלו תקינים
            if (gradeUpdate == null)
            {
                return BadRequest("Invalid grade update data.");
            }

            // חפש את ההגשה על פי ExamId ו-StudentId
            var submission = await _context.ExamSubmissions
                .FirstOrDefaultAsync(s => s.ExamId == examId && s.StudentId == studentId);

            if (submission != null)
            {
                // אם ההגשה קיימת, עדכן את הציון והפידבק
                submission.Grade = gradeUpdate.Grade; // עדכון לשדה Grade
                submission.Feedback = gradeUpdate.Feedback;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Grade updated successfully." });
            }
            else
            {
                // אם ההגשה לא קיימת, צור הגשה חדשה
                var newSubmission = new ExamSubmission
                {
                    ExamId = examId,
                    StudentId = studentId,
                    FilePath = gradeUpdate.FilePath,
                    Grade = gradeUpdate.Grade, // עדכון לשדה Grade
                    Feedback = gradeUpdate.Feedback,
                    SubmissionDate = DateTime.UtcNow // תאריך הגשה נוכחי
                };

                await _context.ExamSubmissions.AddAsync(newSubmission);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(UpdateOrAddExamGrade), new { courseId, examId, studentId }, newSubmission);
            }
        }







    }



}
