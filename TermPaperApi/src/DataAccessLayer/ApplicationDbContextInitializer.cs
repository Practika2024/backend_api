using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer;

public class ApplicationDbContextInitializer(ApplicationDbContext context)
{
    public async Task InitializeAsync()
    {
        await context.Database.MigrateAsync();
    }
}