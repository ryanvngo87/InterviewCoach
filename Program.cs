using interviewCoachAI.Options;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using System.Linq;

namespace interviewCoachAI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddControllers();

            builder.Services.Configure<AzureSpeechOptions>(
                builder.Configuration.GetSection("AzureSpeech"));

            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();
            app.MapGet("/debug/ping", () => "pong");

            app.MapGet("/debug/endpoints", (IEnumerable<EndpointDataSource> sources) =>
            {
                return sources
                    .SelectMany(s => s.Endpoints)
                    .Select(ep =>
                    {
                        if (ep is RouteEndpoint re)
                            return re.RoutePattern.RawText;  
                        return ep.ToString() ?? "(unknown endpoint)";
                    })
                    .ToArray();
            });

            app.Run();
        }
    }
}
