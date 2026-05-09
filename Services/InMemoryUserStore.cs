using CloudCiApi.Models;

namespace CloudCiApi.Services;

public class InMemoryUserStore : IUserStore
{
    // Hard-coded for this lab. Real apps store hashed passwords.
    private static readonly List<User> Users =
    [
        new("alice", "alice123", "admin"),
        new("bob",   "bob456",   "reader"),
    ];

    public User? Validate(string username, string password) =>
        Users.FirstOrDefault(u =>
            string.Equals(u.Username, username, StringComparison.Ordinal) &&
            string.Equals(u.Password, password, StringComparison.Ordinal));
}
