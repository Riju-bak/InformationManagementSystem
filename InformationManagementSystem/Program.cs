using InformationManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace InformationManagementSystem;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        //Register the SchoolContext
        builder.Services.AddDbContext<SchoolContext>(options => options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));

        // AddDatabaseDeveloperPageExceptionFilter provides helpful error info in the development environment
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        var app = builder.Build();

        CreateDbIfNotExists(app);

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

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }

    private static void CreateDbIfNotExists(IHost host)
    {
        //1) Get the database context instance from dependency injection container.
        //2) Call DbInitializer.Initialize(context);
        // 3)Dispose the context when the method completes
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<SchoolContext>();
                DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured creating the DB");
            }
        }
    }
}