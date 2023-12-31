using CourseWebsiteDotNet.Middleware;
using System;
using System.Data.SqlClient;

namespace CourseWebsiteDotNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();
            app.UseSessionCheckMiddleware();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "login",
                    pattern: "login",
                    defaults: new { controller = "Authentication", action = "Index" }
                );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Authentication}/{action=Index}"
                );

                endpoints.MapControllerRoute(
                    name: "courses",
                    pattern: "courses",
                    defaults: new { controller = "Course", action = "Index" }
                );
                endpoints.MapControllerRoute(
                    name: "courses/information",
                    pattern: "courses/information",
                    defaults: new { controller = "Course", action = "Information" }
                );
                endpoints.MapControllerRoute(
                    name: "courses/resource",
                    pattern: "courses/resource",
                    defaults: new { controller = "CourseResource", action = "Resource" }
                );
				endpoints.MapControllerRoute(
					name: "courses/resource/assignment",
					pattern: "courses/resource/assignment",
					defaults: new { controller = "Assignment", action = "Assignment" }
				);
				endpoints.MapControllerRoute(
                    name: "lecturers",
                    pattern: "lecturers",
                    defaults: new { controller = "Lecturer", action = "Index" }
                );

                endpoints.MapControllerRoute(
                    name: "students",
                    pattern: "students",
                    defaults: new { controller = "Student", action = "Index" }
                );

                endpoints.MapControllerRoute(
                    name: "users",
                    pattern: "users",
                    defaults: new { controller = "User", action = "Index" }
                );

                endpoints.MapControllerRoute(
                    name: "subjects",
                    pattern: "subjects",
                    defaults: new { controller = "Subject", action = "Index" }
                );
                
                endpoints.MapControllerRoute(
                    name: "profile/lecturer",
                    pattern: "profile/lecturer/{id?}",
                    defaults: new { controller = "Profile", action = "Index", isLecturer = true }
                );

                endpoints.MapControllerRoute(
                    name: "profile/student",
                    pattern: "profile/student/{id?}",
                    defaults: new { controller = "Profile", action = "Index", isLecturer = false }
                );
            });
            //app.MapControllerRoute(
            //   name: "default",
            //   pattern: "{controller=Home}/{action=Index}/{id?}");

            //app.MapControllerRoute(
            //   name: "Login",
            //   pattern: "{controller=User}/{action=Index}/{id?}");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            app.Run();
        }
    }
}