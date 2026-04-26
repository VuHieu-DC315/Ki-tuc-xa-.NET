using kitucxa.Models;

namespace kitucxa.Service
{
    public interface IAuthService
    {
        User? Login(string username, string password);

        void Register(User user);
    }
}