using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class Context:DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public Context()
        {
            
        }
        public Context(DbContextOptions<Context> options)
            :base(options) 
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Student>()
                 .HasMany(s => s.Courses)
                 .WithMany(c => c.Students)
                 .UsingEntity(j => j.ToTable("StudentCourses"));
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder=>builder.AddConsole())).EnableSensitiveDataLogging();
        }
    }
}
