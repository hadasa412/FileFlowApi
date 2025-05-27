using core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;



namespace FileFlowApi.Data
{
    public class FileFlowDbContextFactory : IDesignTimeDbContextFactory<FileFlowDbContext>
    {

        public FileFlowDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FileFlowDbContext>();
            optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection"),
     new MySqlServerVersion(new Version(8, 0, 34))); // תוודא שהגרסה מתאימה לבסיס הנתונים שלך


            return new FileFlowDbContext(optionsBuilder.Options);
        }

    }
}
