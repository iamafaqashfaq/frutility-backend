using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using frutility_backend.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using frutility_backend.Data.Model;

namespace frutility_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using(var services = host.Services.CreateScope())
            {
                var dbcontext = services.ServiceProvider.GetRequiredService<DataContext>();
                var usermgr = services.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var rolemgr = services.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                dbcontext.Database.Migrate();
                var adminrole = new IdentityRole("Admin");
                if (!dbcontext.Roles.Any())
                {
                    rolemgr.CreateAsync(adminrole).GetAwaiter().GetResult();
                }
                if(!dbcontext.Users.Any(u=>u.UserName == "admin"))
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = "admin@frutility.com",
                        Email = "admin@frutility.com"
                    };
                    var result = usermgr.CreateAsync(adminUser, "password").GetAwaiter().GetResult();
                    usermgr.AddToRoleAsync(adminUser, adminrole.Name).GetAwaiter().GetResult();
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
