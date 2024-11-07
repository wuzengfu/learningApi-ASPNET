using LearningAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(MyDbContext context, IConfiguration configuration) : ControllerBase
    {
        private readonly MyDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            request.Name = request.Name.Trim();
            request.Email = request.Email.Trim();
            request.Password = request.Password.Trim();

            var foundUser = _context.Users
                .FirstOrDefault(x => x.Email == request.Email);

            if (foundUser != null)
            {
                string message = "Email already exists!";
                return BadRequest(new { message });
            }

            var now = DateTime.Now;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User()
            {
                Name = request.Name,
                Email = request.Email,
                Password = passwordHash,
                CreatedAt = now,
                UpdatedAt = now,
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();

            string message = "Email or password is not correct.";
            var foundUser = _context.Users.FirstOrDefault(x => x.Email == request.Email);
            if (foundUser == null)
            {
                return BadRequest(new { message });
            }

            bool verified = BCrypt.Net.BCrypt.Verify(
                request.Password, foundUser.Password);
            if (!verified)
            {
                return BadRequest(new { message });
            }

            // Return user info
            var user = new
            {
                foundUser.Id,
                foundUser.Email,
                foundUser.Name
            };
            string accessToken = CreateToken(foundUser);
            return Ok(new { user, accessToken });
        }

        private string CreateToken(User user)
        {
            string? secret = _configuration.GetValue<string>("Authentication:Secret");
            if (string.IsNullOrEmpty(secret))
            {
                throw new Exception("Secret is required for JWT authentication.");
            }

            int tokenExpiresDays = _configuration.GetValue<int>("Authentication:TokenExpiresDays");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email)
                    ]
                ),
                Expires = DateTime.UtcNow.AddDays(tokenExpiresDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);
            
            return token;
        }
    }
}