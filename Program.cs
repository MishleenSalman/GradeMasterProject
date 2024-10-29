using GradeMasterAPInew.Controllers.DB;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// הוספת שירותי API כלליים לאפליקציה
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // הגדרת Swagger ליצירת תיעוד API

// הגדרת CORS - הגדרת הרשאות גישה ממקורות ספציפיים
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5109", "https://localhost:7012") // הכתובות הספציפיות שמורשות
                          .AllowAnyHeader() // מאפשר כל סוגי הכותרות (headers)
                          .AllowAnyMethod() // מאפשר כל סוגי הבקשות (GET, POST וכו')
                          .AllowCredentials()); // מאפשר שליחה של Cookies ו-Credentials אחרים
});

// הגדרת DbContext - חיבור לבסיס הנתונים באמצעות SqlServer
builder.Services.AddDbContext<GradeMasterDbContext>(options =>
    options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DBGradeMaster;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));

// הגדרת שירותים נוספים (אם יש צורך להוסיף שירותים משלך)
// builder.Services.AddSingleton<ICsvLoader, CsvLoader>(); // דוגמה לשירות שיהיה Singleton
// builder.Services.AddScoped<IGradeCalculationService, GradeCalculationService>(); // דוגמה לשירות שהוא Scoped

var app = builder.Build(); // בניית האפליקציה

// קביעת ההתנהגות של האפליקציה בתהליך הפיתוח
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // שימוש ב-Swagger בתהליך הפיתוח לתיעוד API
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); // יצירת נקודת קצה ל-Swagger
    });
}

app.UseStaticFiles(); // שימוש בקבצים סטטיים (כמו HTML, CSS, תמונות)

// תהליך ניתוב הבקשות
app.UseRouting();
app.UseCors("AllowSpecificOrigin"); // הפעלת CORS עם המדיניות שהוגדרה קודם לכן
app.UseAuthorization(); // הפעלת שירותי הרשאות (אם יש צורך)

// ביצוע הגירות למסד הנתונים בעת עליית האפליקציה
using (var scope = app.Services.CreateScope()) // יצירת Scope של שירותים כדי לקבל גישה ל-DbContext
{
    var services = scope.ServiceProvider; // קבלת ספק השירותים המקומי
    var context = services.GetRequiredService<GradeMasterDbContext>(); // שליפת DbContext
    try
    {
        context.Database.Migrate(); // ביצוע הגירות למסד הנתונים לפי ההגדרות במיגרציות
        // DbInitializer.Initialize(context); // אם יש פונקציה לאתחול בסיס הנתונים, ניתן להפעיל אותה כאן
    }
    catch (Exception ex)
    {
        // טיפול בשגיאות אם תהליך ההגירות נכשל
        var logger = services.GetRequiredService<ILogger<Program>>(); // שליפת שירות לוגים לרישום שגיאות
        logger.LogError(ex, "An error occurred while migrating the database."); // רישום הודעת שגיאה בלוג
    }
}

app.MapControllers(); // מיפוי בקרי ה-API לנתיבי הגישה

app.Run(); // הרצת היישום
