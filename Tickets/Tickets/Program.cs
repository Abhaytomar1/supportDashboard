using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Data;

namespace Tickets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllersWithViews();
            // Add session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";  // Redirect to login page if not authenticated
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Cookie expiration time
                    options.SlidingExpiration = true;  // Reset expiration time if user is active
                });
            builder.Services.AddScoped<ITicketService, TicketService>();

            // Enable session
            builder.Services.AddSession();

            var app = builder.Build();

            app.Use(async (context, next) =>
            {
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";    // for session , authentication and authorization
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "-1";
                await next();
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // Add this line to enable session management
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
