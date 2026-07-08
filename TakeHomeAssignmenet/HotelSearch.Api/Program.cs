// File: HotelSearch.Api/Program.cs
using HotelSearch.Domain.Interfaces;
using HotelSearch.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// REGISTER OUR DATABASE: Singleton means the same dictionary stays alive as long as the API is running
builder.Services.AddSingleton<IHotelRepository, InMemoryHotelRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();