using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ToDoMVC.Models;

namespace ToDoMVC.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<TaskItem> TaskItems { get; set; }
    }
}
