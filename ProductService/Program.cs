using Microsoft.EntityFrameworkCore;
using ProductService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddControllers();



// Đọc connection string từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("ProductDbService");

builder.Services.AddDbContext<ProductServiceContext>(options => options.UseLazyLoadingProxies(false).UseSqlServer(connectionString));



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});



var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseCors("AllowAll"); // phải đặt trước app.UseAuthorization();


app.MapControllers();
app.Run();



