using System.ComponentModel.DataAnnotations;

namespace GradeMasterAPInew.APIModels
{
    public class LoginDTO
    {
        [Required] // מבטיח שהשדה לא יהיה ריק
        [EmailAddress] // מאמת שהקלט הוא כתובת אימייל תקפה
        public string Email { get; set; } = string.Empty; // כתובת האימייל

        [Required] // מבטיח שהשדה לא יהיה ריק
        [DataType(DataType.Password)] // מציין שזה שדה של סיסמה
        public string Password { get; set; } = string.Empty; // הסיסמה
    }
}
