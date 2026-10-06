using System.Text;
using DEMO_SEMINAR_API.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Cấu hình Swagger UI có nút Authorize để nhập Bearer Token
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth Demo API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập trực tiếp chuỗi JWT Token vào đây (Swagger sẽ tự động gắn tiền tố 'Bearer ')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// [A] CẤU HÌNH AUTHENTICATION (XÁC THỰC DANH TÍNH)
var secretKey = "Khoa_Bi_Mat_Cuc_Ky_An_Toan_Cho_Seminar_2026!";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "SeminarAuthServer",
        ValidAudience = "SeminarClients",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// [B] CẤU HÌNH AUTHORIZATION (POLICIES)
builder.Services.AddAuthorization(options =>
{
    // Policy theo Role
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

    // Policy theo Custom Requirement (Tuổi >= 18)
    options.AddPolicy("AtLeast18YearsOld", policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(18)));

    // Policy theo Resource
    options.AddPolicy("DocumentOwnerPolicy", policy =>
        policy.Requirements.Add(new DocumentOwnerRequirement()));
});

// [C] ĐĂNG KÝ CÁC HANDLER VÀO DI CONTAINER
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, DocumentOwnerHandler>();

var app = builder.Build();

// Cấu hình Middleware Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// [D] THỨ TỰ MIDDLEWARE BẮT BUỘC: AUTHENTICATION TRƯỚC, AUTHORIZATION SAU!
app.UseAuthentication(); // 1. Xác thực: Biết bạn là ai -> Tạo User
app.UseAuthorization();  // 2. Phân quyền: Dựa trên User vừa tạo -> Cấp/chặn quyền

app.MapControllers();
app.Run();
