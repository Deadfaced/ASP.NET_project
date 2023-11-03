using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcBlog.Areas.Identity.Data;
using MvcBlog.Data;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Create a new web application builder with the provided command-line arguments
        var builder = WebApplication.CreateBuilder(args);

        // Get the connection string from the configuration
        var connectionString =
            builder.Configuration.GetConnectionString("JapanWandererContext")
            ?? throw new InvalidOperationException(
                "Connection string 'JapanWandererContext' not found."
            );

        // Add the DbContext to the service collection
        builder.Services.AddDbContext<MvcBlogContext>(
            options => options.UseSqlite(connectionString)
        );

        // Add the default identity system configuration to the service collection
        builder.Services
            .AddDefaultIdentity<MvcBlogUser>(
                options => options.SignIn.RequireConfirmedAccount = false
            )
            .AddRoles<IdentityRole>() // Add roles support
            .AddEntityFrameworkStores<MvcBlogContext>(); // Add EF support

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        // Build the application
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        // Create a new scope to be able to use scoped services
        using (var scope = app.Services.CreateScope())
        {
            // Get the RoleManager service
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Admin", "User"};

            // Ensure the roles exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        // Create a new scope to be able to use scoped services
        using (var scope = app.Services.CreateScope())
        {
            // Get the UserManager service
            var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<MvcBlogUser>>();

            string email = "admin@admin.com";
            string password = "Admin123!";

            // Ensure the admin user exists
            if (await UserManager.FindByEmailAsync(email) == null)
            {
                var user = new MvcBlogUser { UserName = email, Email = email, FirstName = "Admin", LastName = "Admin"};

                await UserManager.CreateAsync(user, password);

                await UserManager.AddToRoleAsync(user, "Admin");
            }
        }

        // Run the application
        app.Run();
    }
}