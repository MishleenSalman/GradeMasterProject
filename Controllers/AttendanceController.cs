using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradeMasterAPInew.APIModels;
using GradeMasterAPInew.Controllers.DB.DBModels;
using GradeMasterAPInew.Controllers.DB;
using Microsoft.EntityFrameworkCore;

namespace GradeMasterAPInew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly GradeMasterDbContext _context;

        public AttendanceController(GradeMasterDbContext context)
        {
            _context = context;
        }

        // GET: api/attendance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceDTO>>> Get()
        {
            var attendances = await _context.Attendances.ToListAsync();
            var attendanceDTOs = attendances.Select(a => new AttendanceDTO
            {
                AttendanceId = a.AttendanceId,
                RoomNumber = a.RoomNumber,
                Start = a.Start,
                DurationMinutes = a.DurationMinutes,
                Status = a.Status,
                Notes = a.Notes,
                StudentId = a.StudentId,
                CourseId = a.CourseId,
                TeacherId = a.TeacherId
            });

            return Ok(attendanceDTOs);
        }

        // GET api/attendance/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AttendanceDTO>> Get(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            var attendanceDTO = new AttendanceDTO
            {
                AttendanceId = attendance.AttendanceId,
                RoomNumber = attendance.RoomNumber,
                Start = attendance.Start,
                DurationMinutes = attendance.DurationMinutes,
                Status = attendance.Status,
                Notes = attendance.Notes,
                StudentId = attendance.StudentId,
                CourseId = attendance.CourseId,
                TeacherId = attendance.TeacherId
            };

            return Ok(attendanceDTO);
        }

        // POST api/attendance
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AttendanceDTO attendanceDTO)
        {
            var attendance = new Attendance
            {
                RoomNumber = attendanceDTO.RoomNumber,
                Start = attendanceDTO.Start,
                DurationMinutes = attendanceDTO.DurationMinutes,
                Status = attendanceDTO.Status,
                Notes = attendanceDTO.Notes,
                StudentId = attendanceDTO.StudentId,
                CourseId = attendanceDTO.CourseId,
                TeacherId = attendanceDTO.TeacherId
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync(); // שמירת השינויים במסד הנתונים

            return CreatedAtAction(nameof(Get), new { id = attendance.AttendanceId }, attendance);
        }

        // PUT api/attendance/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] AttendanceDTO attendanceDTO)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            attendance.RoomNumber = attendanceDTO.RoomNumber;
            attendance.Start = attendanceDTO.Start;
            attendance.DurationMinutes = attendanceDTO.DurationMinutes;
            attendance.Status = attendanceDTO.Status;
            attendance.Notes = attendanceDTO.Notes;
            attendance.StudentId = attendanceDTO.StudentId;
            attendance.CourseId = attendanceDTO.CourseId;
            attendance.TeacherId = attendanceDTO.TeacherId;

            await _context.SaveChangesAsync(); // שמירת השינויים במסד הנתונים

            return NoContent();
        }

        // DELETE api/attendance/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync(); // שמירת השינויים במסד הנתונים

            return NoContent();
        }
    }
}
