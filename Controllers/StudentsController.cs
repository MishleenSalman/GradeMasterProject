using Microsoft.AspNetCore.Mvc;
using GradeMasterAPInew.APIModels;
using System.Collections.Generic;
using System.Linq;
using GradeMasterAPInew.Controllers.DB.DBModels;
using GradeMasterAPInew.Controllers.DB;

namespace GradeMasterAPInew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly GradeMasterDbContext _context; // יש להחליף בשם הקונטקסט שלך

        public StudentsController(GradeMasterDbContext context)
        {
            _context = context;
        }

        // GET: api/<StudentsController>
        [HttpGet]
        public ActionResult<IEnumerable<StudentDTO>> Get()
        {
            var students = _context.Students
                .Select(s => new StudentDTO
                {
                    StudentId = s.StudentId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    DateBirth = s.DateBirth,
                    Gender = s.Gender,
                    PhoneNumber = s.PhoneNumber,
                    Address = s.Address,
                    Email = s.Email,
                    EnrollmentDate = s.EnrollmentDate
                }).ToList();

            return Ok(students);
        }

        // GET api/<StudentsController>/5
        [HttpGet("{id}")]
        public ActionResult<StudentDTO> Get(int id)
        {
            var student = _context.Students
                .Where(s => s.StudentId == id)
                .Select(s => new StudentDTO
                {
                    StudentId = s.StudentId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    DateBirth = s.DateBirth,
                    Gender = s.Gender,
                    PhoneNumber = s.PhoneNumber,
                    Address = s.Address,
                    Email = s.Email,
                    EnrollmentDate = s.EnrollmentDate
                })
                .FirstOrDefault();

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // POST api/<StudentsController>
        [HttpPost]
        public ActionResult<StudentDTO> Post([FromBody] StudentDTO studentDto)
        {
            if (studentDto == null)
            {
                return BadRequest();
            }

            // יצירת אובייקט סטודנט חדש
            var student = new Student
            {
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                DateBirth = studentDto.DateBirth,
                Gender = studentDto.Gender,
                PhoneNumber = studentDto.PhoneNumber,
                Address = studentDto.Address,
                Email = studentDto.Email,
                EnrollmentDate = studentDto.EnrollmentDate
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            studentDto.StudentId = student.StudentId; // קביעת מזהה הסטודנט שנוצר
            return CreatedAtAction(nameof(Get), new { id = student.StudentId }, studentDto);
        }

        // PUT api/<StudentsController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] StudentDTO studentDto)
        {
            if (studentDto == null || studentDto.StudentId != id)
            {
                return BadRequest();
            }

            var student = _context.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }

            // עדכון נתוני הסטודנט
            student.FirstName = studentDto.FirstName;
            student.LastName = studentDto.LastName;
            student.DateBirth = studentDto.DateBirth;
            student.Gender = studentDto.Gender;
            student.PhoneNumber = studentDto.PhoneNumber;
            student.Address = studentDto.Address;
            student.Email = studentDto.Email;
            student.EnrollmentDate = studentDto.EnrollmentDate;

            _context.Students.Update(student);
            _context.SaveChanges();

            return NoContent(); // מחזיר סטטוס 204 No Content
        }

        // DELETE api/<StudentsController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            _context.SaveChanges();

            return NoContent(); // מחזיר סטטוס 204 No Content
        }
    }
}
