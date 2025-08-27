
using AutoMapper;
using CustomerTableTest.BLL;
using CustomerTableTest.DAL;
using CustomerTableTest.DAL.Repositories;
using CustomerTableTest.Models;
using CustomerTableTest.Models.Validation;
using CustomerTableTestAPI.Common.Middlewares;
//using CustomerTableTestAPI.Middlewares;
using CustomerTableTestAPI.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

// 🔷 Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔷 Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 🔷 Repositories & Services
//builder.Services.AddScoped(typeof(CustomerTableTest.DAL.Repositories.IBaseRepository<>),
//                          typeof(CustomerTableTest.DAL.Repositories.BaseRepository<>));
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

builder.Services.AddScoped<CustomerTableTest.BLL.Services.ICustomerService,
                           CustomerTableTest.BLL.Services.CustomerService>();
// Controllers + FluentValidation
builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
})
.AddJsonOptions(_ => { });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CustomerTableTest.Models.Validation.CustomerValidator>();

builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
// 🔷 JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// 🔷 Swagger + JWT Support in Swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
// 🔷 Controllers
//builder.Services.AddControllers();
//builder.Services
//    .AddControllers(options =>
//    {
//        // هنمنع 400 التلقائية عشان الميدلوير هو اللي ينسّق الرد
//        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
//    })
//    .AddJsonOptions(o => { /* اختياري لتنسيقات */ });

//builder.Services.AddFluentValidationAutoValidation();
//builder.Services.AddValidatorsFromAssemblyContaining<CustomerValidator>();

//// مهم: امنعي الـ 400 التلقائي من ApiBehavior
//builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
//{
//    options.SuppressModelStateInvalidFilter = true;
//});
//builder.Services.AddControllers()
//    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CustomerDtoValidator>());


// 🔷 AutoMapper (هنا قبل الـ Build)
//builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//builder.Services.AddAutoMapper(cfg =>
//{
//    // ضيفي البروفايل صراحةً
//    cfg.AddProfile<CustomerProfile>();
//});



// 🔷 Build App
var app = builder.Build();

// 🔷 Development tools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// 3) (اختياري مفيد) تحقّق إعدادات AutoMapper في الـ Dev
//if (app.Environment.IsDevelopment())
//{
//    using var scope = app.Services.CreateScope();
//    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

//    // لو في Map ناقص/غلط هيفشل الإقلاع برسالة واضحة
//    mapper.ConfigurationProvider.AssertConfigurationIsValid();
//}
//if (app.Environment.IsDevelopment())
//{
//    using var scope = app.Services.CreateScope();
//    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
//    try
//    {
//        mapper.ConfigurationProvider.AssertConfigurationIsValid();
//    }
//    catch (AutoMapperConfigurationException ex)
//    {
//        Console.WriteLine("=== AutoMapper config error ===");
//        Console.WriteLine(ex.ToString());   // هيطبع النوعين والممّبر اللي بايظ
//        throw; // سيبّيها تفشل عشان نصلح، أو علّقيها مؤقتًا لو عايزة تشغّلي API بسرعة
//    }
//}
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
// 🔷 Auth middleware

//app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<CustomerTableTestAPI.Common.Middlewares.ExceptionHandlingMiddleware>();


// 🔷 Controller Routing
app.MapControllers();

//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//builder.Services.AddAutoMapper(typeof(Program));
//builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

app.Run();



//using CustomerTableTest.BLL;
//using CustomerTableTest.DAL;
//using CustomerTableTest.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using Microsoft.OpenApi.Models;


//var builder = WebApplication.CreateBuilder(args);

//// Add DbContext
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// Add Identity
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

//// Add Repositories and Services
//builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
//builder.Services.AddScoped<ICustomerService, CustomerService>();

//// Add Controllers
//builder.Services.AddControllers();

//// Add Swagger
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer API", Version = "v1" });

//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                },
//                Scheme = "oauth2",
//                Name = "Bearer",
//                In = ParameterLocation.Header,
//            },
//            new List<string>()
//        }
//    });
//});

//// 🟢 Jwt Configuration (لازم فوق قبل Build)
//var jwtSettings = builder.Configuration.GetSection("Jwt");
//var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = false;
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtSettings["Issuer"],
//        ValidAudience = jwtSettings["Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(key)
//    };
//});


//// Build
//var app = builder.Build();

//// Swagger
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//// Authentication & Authorization
//app.UseAuthentication();
//app.UseAuthorization();

//// Controllers
//app.MapControllers();

//app.Run();
