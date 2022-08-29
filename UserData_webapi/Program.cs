using UserData_webapi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IUserDataRepository, UserDataRepository>();
builder.Services.AddSingleton<IUserCardRepository, UserCardRepository>();
builder.Services.AddSingleton<IUserLogRepository, UserLogRepository>();
builder.Services.AddSingleton<ILineJobRespository, LineJobRepository>();
builder.Services.AddSingleton<ILineBotManageRespository, LineManageRepository>();
builder.Services.AddSingleton<IImageRepository, ImageRepository>();
builder.Services.AddSingleton<IRechalRepository, RachelRepository>();
builder.Services.AddSingleton<IFaceRepository, FaceRepository>();
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors("MyPolicy");
app.Run();
