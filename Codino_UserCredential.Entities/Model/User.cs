namespace Codino_UserCredential.Entities.Model;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = "";

    public string Email { get; set; } = "";

    public string PasswordHash { get; set; } = "";

    public int Level { get; set; } = 1;

    public int Xp { get; set; } = 0;

    public string? MascotImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // public ICollection<UserAchievement> UserAchievements { get; set; } = new();
}