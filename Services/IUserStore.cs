using CloudCiApi.Models;

namespace CloudCiApi.Services;

public interface IUserStore
{
    // Returns the user when credentials match, null otherwise.
    User? Validate(string username, string password);
}
