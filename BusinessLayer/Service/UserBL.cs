using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDatabase;

        public UserBL(IUserRL userRL, IConfiguration configuration, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor, IConnectionMultiplexer redisConnection)
        {
            _userRL = userRL;
            _configuration = configuration;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
            _redisConnection = redisConnection;
            _redisDatabase = redisConnection.GetDatabase(); // Getting Redis database instance
        }

        public ResponseModel<UserEntity> Register(UserRegisterRequestModel user)
        {
            return _userRL.Register(user);
        }

        public ResponseModel<UserEntity> Login(UserLoginRequestModel user)
        {
            var result = _userRL.Login(user);

            if (result == null || result.Data == null)
            {
                return null;
            }

            string token = GenerateToken(result.Data);

            // Store the JWT token in Redis (using the UserId as the key)
            _redisDatabase.StringSet($"session:{result.Data.Id}", token, TimeSpan.FromMinutes(60)); // Setting expiration to 60 minutes

            return new ResponseModel<UserEntity>
            {
                Data = result.Data,
                Message = "Login successful",
                StatusCode = 200,
                Success = true,
                Token = token
            };
        }

        public string GenerateToken(UserEntity user)
        {
            if (string.IsNullOrEmpty(_configuration["Jwt:Key"]) ||
                string.IsNullOrEmpty(_configuration["Jwt:Issuer"]) ||
                string.IsNullOrEmpty(_configuration["Jwt:Audience"]))
            {
                throw new InvalidOperationException("JWT Configuration values are missing");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"] ?? "AddressBookAPI"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("Email", user.Email)
            };

            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var signin = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signin
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AsyncResponseModel<string>> ForgotPassword(ForgotPasswordRequestModel request)
        {
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            var subject = "Password Reset Request";
            var message = $"To reset your password, please use the OTP below:\n\n{otp}";

            // Send email without including Action in response
            await _emailSender.SendEmailAsync(request.Email, subject, message);

            // Store OTP in Redis temporarily (valid for 10 minutes)
            _redisDatabase.StringSet($"otp:{request.Email}", otp.ToString(), TimeSpan.FromMinutes(10)); // Setting OTP expiration

            return new AsyncResponseModel<string>(
                "Your OTP is the token",
                "Email sent successfully",
                200,
                true,
                "Check your email"
            );
        }

        public ResponseModel<string> ResetPassword(ResetPasswordRequestModel request)
        {
            var storedOtp = _redisDatabase.StringGet($"otp:{request.Email}"); // Get OTP from Redis

            if (storedOtp.IsNullOrEmpty)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "OTP expired or not found.",
                    StatusCode = 400
                };
            }

            if (storedOtp != request.Otp.ToString())
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "Invalid OTP.",
                    StatusCode = 400
                };
            }

            return _userRL.ResetPassword(request);
        }
    }
}
