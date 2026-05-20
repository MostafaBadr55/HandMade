using HandMade.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Infrastructure.Helpers
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("data source=DESKTOP-LGDSPJL\\MSONA355;initial catalog=HandMade;integrated security=True;trustservercertificate=True;MultipleActiveResultSets=True;");
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
