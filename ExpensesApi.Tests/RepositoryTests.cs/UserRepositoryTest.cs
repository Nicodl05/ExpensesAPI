using Microsoft.EntityFrameworkCore;
using ExpensesAPI.Domain;
using ExpensesAPI.Data;
using NSubstitute;
using ExpensesAPI.Exceptions;

namespace ExpensesAPI.Repositories.Tests
{


    public class UserRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly AppDbContext _context;
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(_options);
            _userRepository = new UserRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        // ******************
        // ******************
        // GET UserById Tests
        // ******************
        // ******************
        [Fact]
        public void GetUserByIdShouldReturnUserWhenUserExists()
        {
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            _context.Users.Add(user);
            _context.SaveChanges();

            var result = _userRepository.GetUserById(user.Id);
            Assert.NotNull(result);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.Currency, result.Currency);
        }

        [Fact]
        public void GetUserByIdShouldReturnNullWhenUserDoesNotExist()
        {
            var result = _userRepository.GetUserById(Guid.NewGuid());
            Assert.Null(result);
        }

        [Fact]
        public void GetUserByDetails_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var result = _userRepository.GetUserByDetails("Unknown", "User", Currency.EUR);

            Assert.Null(result);
        }

        [Fact]
        public void GetUserByDetails_ShouldReturnUser_WhenUserExists()
        {
            var user = new User { FirstName = "Bruce", LastName = "Wayne", Currency = Currency.USD };
            _context.Users.Add(user);
            _context.SaveChanges();

            var result = _userRepository.GetUserByDetails("Bruce", "Wayne", Currency.USD);

            Assert.NotNull(result);
            Assert.Equal("Bruce", result.FirstName);
            Assert.Equal("Wayne", result.LastName);
            Assert.Equal(Currency.USD, result.Currency);
        }

        // ******************
        // ******************
        // Add Users Tests
        // ******************
        // ******************

        [Fact]
        public void AddUserShouldAddUserToDatabase()
        {
            var user = new User { FirstName = "Natasha", LastName = "Romanova", Currency = Currency.RUB };

            _userRepository.AddUser(user);
            _context.SaveChanges();

            var retrievedUser = _context.Users.FirstOrDefault(u => u.FirstName == "Natasha" && u.LastName == "Romanova");
            Assert.NotNull(retrievedUser);
            Assert.Equal(Currency.RUB, retrievedUser.Currency);
        }
        [Fact]
        public void AddUserShouldThrowDuplicateUserExceptionWhenUserAlreadyExists()
        {
            // Arrange
            var user = new User { FirstName = "Natasha", LastName = "Romanova", Currency = Currency.RUB };
            _userRepository.AddUser(user);
            _context.SaveChanges();

            // Act & Assert
            var exception = Assert.Throws<DuplicateUserException>(() => _userRepository.AddUser(user));
            Assert.Equal("A user with the same details already exists: Natasha Romanova", exception.Message);
        }
    }
}