using ExpensesAPI.Domain;
using ExpensesAPI.Data;

namespace ExpensesAPI.Repositories
{
    public interface IUserRepository
    {
        User? GetUserById(Guid userId);

        User? GetUserByDetails(string firstName, string lastName, Currency currency);
        void AddUser(User user);

    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public User? GetUserByDetails(string firstName, string lastName, Currency currency)
        {
            return _context.Users.FirstOrDefault(u => u.FirstName == firstName && u.LastName == lastName && u.Currency == currency);
        }
        public User? GetUserById(Guid userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }
        public void AddUser(User user)
        {
            var existingUser = _context.Users
        .FirstOrDefault(u => u.FirstName == user.FirstName && u.LastName == user.LastName && u.Currency == user.Currency);

            if (existingUser != null)
            {
                throw new DuplicateUserException($"A user with the same details already exists: {user.FirstName} {user.LastName}");
            }

            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
