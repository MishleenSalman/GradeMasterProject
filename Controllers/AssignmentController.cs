using GradeMasterAPInew.APIModels;
using GradeMasterAPInew.Controllers.DB;
using GradeMasterAPInew.Controllers.DB.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GradeMasterAPInew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentController : ControllerBase
    {
        private readonly GradeMasterDbContext _context;

        public AssignmentController(GradeMasterDbContext context)
        {
            _context = context;
        }

        // GET: api/assignments/title/{title}
        [HttpGet("title/{title}")]
        public ActionResult<int?> GetAssignmentIdByTitle(string title)
        {
            // המרת הטייטל לאותיות קטנות
            var titleLower = title.ToLower();

            // חפש מטלה לפי TITLE תוך השוואה לא רגישה לאותיות
            var assignment = _context.Assignments
                .FirstOrDefault(a => a.Title.ToLower() == titleLower); // המרה לקטן להשוואת מחרוזות

            if (assignment == null)
            {
                return NotFound(); // אם לא נמצאה מטלה, החזר 404
            }

            return Ok(assignment.AssignmentId); // החזר את ה-ID של המטלה
        }

        // פעולה לקבלת כל המטלות של קורס מסוים
        [HttpGet("GetAssignmentsByCourse/{courseId}")]
        public IActionResult GetAssignmentsByCourse(int courseId)
        {
            var assignments = _context.Assignments
                .Where(a => a.CourseId == courseId)
                .Select(a => new AssignmentDTO
                {
                    AssignmentId = a.AssignmentId,
                    Title = a.Title,
                    Description = a.Description,
                    DueDate = a.DueDate,
                    CourseId = a.CourseId
                })
                .ToList();

            if (assignments == null || assignments.Count == 0)
            {
                return NotFound("No assignments found for this course.");
            }

            return Ok(assignments);
        }

        // פעולה להוספת מטלה חדשה
        [HttpPost]
        public IActionResult CreateAssignment([FromBody] AssignmentDTO assignmentDto)
        {
            if (assignmentDto == null)
            {
                return BadRequest("Invalid assignment data.");
            }

            // יצירת אובייקט Assignment מה-DTO
            var assignment = new Assignment
            {
                Title = assignmentDto.Title,
                Description = assignmentDto.Description,
                DueDate = assignmentDto.DueDate,
                CourseId = assignmentDto.CourseId
            };

            // שמירת המטלה החדשה במסד הנתונים
            _context.Assignments.Add(assignment);
            _context.SaveChanges();

            return Ok(new { message = "Assignment created successfully", AssignmentId = assignment.AssignmentId });
        }

        
        // פעולה לעדכון מטלה קיימת
        [HttpPut("{assignmentId}")]
        public IActionResult UpdateAssignment(int assignmentId, [FromBody] AssignmentDTO assignmentDto)
        {
            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignmentId == assignmentId);
            if (assignment == null)
            {
                return NotFound("Assignment not found.");
            }

            // עדכון נתוני המטלה
            assignment.Title = assignmentDto.Title;
            assignment.Description = assignmentDto.Description;
            assignment.DueDate = assignmentDto.DueDate;

            _context.SaveChanges();

            return Ok(new { message = "Assignment updated successfully." });
        }

        // פעולה למחיקת מטלה
        [HttpDelete("{assignmentId}")]
        public IActionResult DeleteAssignment(int assignmentId)
        {
            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignmentId == assignmentId);
            if (assignment == null)
            {
                return NotFound("Assignment not found.");
            }

            _context.Assignments.Remove(assignment);
            _context.SaveChanges();

            return Ok(new { message = "Assignment deleted successfully." });
        }


    }
}
