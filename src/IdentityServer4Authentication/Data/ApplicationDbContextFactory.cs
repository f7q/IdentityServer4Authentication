using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace IdentityServer4Authentication.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var db = configuration.GetConnectionString("db");
            var defaultConnection = configuration.GetConnectionString("DefaultConnection");
            if (db.Equals("sqlite"))
            {
                builder.UseSqlite(defaultConnection);

            }
            if (db.Equals("sqlserver"))
            {
                builder.UseSqlServer(defaultConnection);
            }
            if (db.Equals("psql"))
            {
                builder.UseNpgsql(defaultConnection);
            }

            return new ApplicationDbContext(builder.Options);
        }
    }
}
