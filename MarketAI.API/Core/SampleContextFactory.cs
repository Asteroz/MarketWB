using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MarketAI.API.Core
{
    public class SampleContextFactory : IDesignTimeDbContextFactory<APIDBContext>
    {
        public APIDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<APIDBContext>();

            // получаем конфигурацию из файла appsettings.json
            ConfigurationBuilder builder = new ConfigurationBuilder();
            IConfigurationRoot config = builder.Build();

            optionsBuilder.UseMySql(
                         "server=localhost;" +
                         "user=root;" +
                         "password=H2c7V7p6;" +
                         "port=3306;" +
                         "database=marketwb;" +
                         "Convert Zero Datetime=true;",
                         new MySqlServerVersion(new Version(8, 0, 11))
                     );
            return new APIDBContext(optionsBuilder.Options);
        }
    }
}
