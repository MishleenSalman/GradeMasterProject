using GradeMasterAPInew.Controllers.DB;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ����� ������ API ������ ���������
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // ����� Swagger ������ ����� API

// ����� CORS - ����� ������ ���� ������� ��������
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5109", "https://localhost:7012") // ������� ��������� �������
                          .AllowAnyHeader() // ����� �� ���� ������� (headers)
                          .AllowAnyMethod() // ����� �� ���� ������ (GET, POST ���')
                          .AllowCredentials()); // ����� ����� �� Cookies �-Credentials �����
});

// ����� DbContext - ����� ����� ������� ������� SqlServer
builder.Services.AddDbContext<GradeMasterDbContext>(options =>
    options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DBGradeMaster;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));

// ����� ������� ������ (�� �� ���� ������ ������� ����)
// builder.Services.AddSingleton<ICsvLoader, CsvLoader>(); // ����� ������ ����� Singleton
// builder.Services.AddScoped<IGradeCalculationService, GradeCalculationService>(); // ����� ������ ���� Scoped

var app = builder.Build(); // ����� ���������

// ����� �������� �� ��������� ������ ������
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // ����� �-Swagger ������ ������ ������ API
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); // ����� ����� ��� �-Swagger
    });
}

app.UseStaticFiles(); // ����� ������ ������ (��� HTML, CSS, ������)

// ����� ����� ������
app.UseRouting();
app.UseCors("AllowSpecificOrigin"); // ����� CORS �� �������� ������� ���� ���
app.UseAuthorization(); // ����� ������ ������ (�� �� ����)

// ����� ������ ���� ������� ��� ����� ���������
using (var scope = app.Services.CreateScope()) // ����� Scope �� ������� ��� ���� ���� �-DbContext
{
    var services = scope.ServiceProvider; // ���� ��� �������� ������
    var context = services.GetRequiredService<GradeMasterDbContext>(); // ����� DbContext
    try
    {
        context.Database.Migrate(); // ����� ������ ���� ������� ��� ������� ���������
        // DbInitializer.Initialize(context); // �� �� ������� ������ ���� �������, ���� ������ ���� ���
    }
    catch (Exception ex)
    {
        // ����� ������� �� ����� ������� ����
        var logger = services.GetRequiredService<ILogger<Program>>(); // ����� ����� ����� ������ ������
        logger.LogError(ex, "An error occurred while migrating the database."); // ����� ����� ����� ����
    }
}

app.MapControllers(); // ����� ���� �-API ������ �����

app.Run(); // ���� ������
