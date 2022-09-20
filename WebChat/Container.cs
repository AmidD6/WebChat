using Microsoft.EntityFrameworkCore;

public class User 
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Password { get; set; } = "";
    public int Role { get; set; }
/*    public User(string name, string password, int role)
    {
        Name = name;
        Password = password;
        Role = role;
    }*/   
}

public class Friend {
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
}

public class Message {
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public DateTime DateTime { get; set; }
    public int FriendId { get; set; }
}

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Friend> Friends { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();   // создаем базу данных при первом обращении
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "admin", Password = "admin", Role = 1 },
                new User { Id = 2, Name = "moder1", Password = "moder1", Role = 2 },
                new User { Id = 3, Name = "user1", Password = "user1", Role = 3 }
        );
    }
}