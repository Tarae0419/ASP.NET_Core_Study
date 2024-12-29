using Microsoft.EntityFrameworkCore;
using ServerStudy.Models;

namespace ServerStudy.DataBase;

public class UserData : DbContext
{
    public UserData(DbContextOptions<UserData> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}