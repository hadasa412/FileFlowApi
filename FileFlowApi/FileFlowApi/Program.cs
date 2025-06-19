using Amazon;
using Amazon.S3;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using FileFlowApi.Data;
using FileFlowApi.Services;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;
using FILEFLOW.Core.IServices;
using FILEFLOW.Core.IRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FileFlowApi.IREPOSITORY;
using FileFlowApi.SERVICES;
using core.IServices;
using data.Repositories;
using core.IRepositories;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Security.Claims;
using service.services;



var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();


// הוספת CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:5173", "http://localhost:5175", "https://fileflow-react.onrender.com")  // הכתובת של React
              .AllowAnyMethod()  // מאפשר כל שיטה (GET, POST וכו')
              .AllowAnyHeader()  // מאפשר כל כותרת
              .AllowCredentials(); // אם נדרש תיעוד נכנס (cookies, headers, וכו')
    });
});

// קריאת פרטי ה-AWS מתוך הקונפיגורציה (באמצעות appsettings.json)
var awsOptions = builder.Configuration.GetSection("AWS");

// יצירת פרטי חיבור ל-S3 מתוך הקונפיגורציה
var accessKey = awsOptions["AccessKey"] ?? throw new Exception("AWS_ACCESS_KEY_ID is missing");
var secretKey = awsOptions["SecretKey"] ?? throw new Exception("AWS_SECRET_ACCESS_KEY is missing");
var region = RegionEndpoint.GetBySystemName(awsOptions["Region"]);

var awsCredentials = new BasicAWSCredentials(accessKey, secretKey);

builder.Services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(awsCredentials, region));

// הוספת DbContext (MySQL) עם חבילת Pomelo
builder.Services.AddDbContextPool<FileFlowDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")),
        optionsBuilder => optionsBuilder.MigrationsAssembly("FileFlowApi") // שם הפרויקט הראשי (ה־API)
    )
);

// הוספת Authentication עם JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

// הוספת שירותים (DI)
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReportService, ReportService>();  // הוספת שירות הדוחות

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<FileUploadOperationFilter>();
builder.Services.AddScoped<SwaggerFileUploadSchemaFilter>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IAIService, AIService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddSingleton<IS3Service, S3Service>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();

// הוספת שירותי API Controllers
builder.Services.AddControllers();

// הגדרת Swagger עם תמיכה בהעלאת קבצים
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FileFlow API", Version = "v1" });
    c.OperationFilter<FileUploadOperationFilter>(); // תמיכה בהעלאת קבצים
    c.SchemaFilter<SwaggerFileUploadSchemaFilter>(); // שים לב לכך שהפילטר נרשם כאן
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// בניית האפליקציה
var app = builder.Build();

// בדיקת חיבור למסד הנתונים עם EnsureCreated() במקום Migrate()
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FileFlowDbContext>();

    try
    {
        dbContext.Database.Migrate(); // זה יבטיח שמיגרציות יתבצעו אם צריך
        Console.WriteLine("Database connection established successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error connecting to database: " + ex.Message);
    }
}

// קביעת תצורת ה-HTTP Pipeline
app.UseHttpsRedirection();

app.UseCors("AllowReactApp"); 
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Ok("FileFlow API is running"));

// הפעלת השרת
app.Run();
