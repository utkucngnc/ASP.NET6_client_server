using logServer.Data;
using logServer.Services;
using Microsoft.EntityFrameworkCore;

namespace logServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Console.WriteLine("Server is initializing...");
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<logDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("LogDB")));
            builder.Services.AddHostedService<BackgroundApplication>();

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
            app.Run();
        }
    }
}