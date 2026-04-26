using kitucxa.Data;
using kitucxa.Models;

namespace kitucxa.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public User? Login(string username, string password)
        {
            return _context.User.FirstOrDefault(u => u.Name == username && u.Password == password);
        }

        public void Register(RegisterVm user)
        {
            var userTamThoi = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Role = "User"
            };
            _context.User.Add(userTamThoi);
            _context.SaveChanges();
        }
    }
}