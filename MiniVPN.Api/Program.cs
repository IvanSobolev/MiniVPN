using Microsoft.EntityFrameworkCore;
using UserChecker.Server.Model;
using UserChecker.Server.Service.Implimentations;
using UserChecker.Server.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:63343") // Укажите ваш источник
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite());

var optionBuilder = new DbContextOptionsBuilder<DataContext>();
optionBuilder.UseSqlite(builder.Configuration.GetConnectionString("SQLiteData"));

var dataContext = new DataContext(optionBuilder.Options);

builder.Services.AddSingleton<IUserManager>(provider =>
{
    dataContext.Database.EnsureCreated();
    IUserRepository userRepository = new UserRepository(dataContext);
    IUserManager userManager = new UserManager(userRepository);
    return userManager;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.MapControllers();

app.Run();