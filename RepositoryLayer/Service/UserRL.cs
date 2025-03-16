using System;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ModelLayer.Model;
using Newtonsoft.Json;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        private readonly AddressBookContext addressBookContext;
        private readonly EmailValidator emailValidator;
        private readonly IDistributedCache redisCache;
        private readonly ILogger<UserRL> logger;  // Inject logging

        public UserRL(AddressBookContext _addressBookContext, EmailValidator _emailValidator,
                      IDistributedCache _redisCache, ILogger<UserRL> _logger)
        {
            addressBookContext = _addressBookContext;
            emailValidator = _emailValidator;
            redisCache = _redisCache;
            logger = _logger;  // Assign logger
        }

        public ResponseModel<UserEntity> Register(UserRegisterRequestModel user)
        {
            if (!emailValidator.IsValidEmail(user.Email))
            {
                logger.LogWarning("Invalid email format for registration: {Email}", user.Email);
                return new ResponseModel<UserEntity>
                {
                    Data = null,
                    Success = false,
                    Message = "Invalid email format.",
                    StatusCode = 400 // Bad Request
                };
            }

            var existingUser = addressBookContext.Users.FirstOrDefault(g => g.Email == user.Email);
            if (existingUser != null)
            {
                logger.LogWarning("User already exists with email: {Email}", user.Email);
                return new ResponseModel<UserEntity>
                {
                    Data = null,
                    Success = false,
                    Message = "User already exists.",
                    StatusCode = 409 // Conflict
                };
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            var newUser = new UserEntity
            {
                Name = user.Name,
                Email = user.Email,
                IsAdmin = false,
                Password = hashedPassword
            };
            addressBookContext.Users.Add(newUser);
            addressBookContext.SaveChanges();

            // Cache user data in Redis
            redisCache.SetString($"user:{newUser.Email}", JsonConvert.SerializeObject(newUser));
            logger.LogInformation("User registered successfully: {Email}", newUser.Email);

            return new ResponseModel<UserEntity>
            {
                Data = newUser,
                Success = true,
                Message = "User Registered Successfully.",
                StatusCode = 200 // OK
            };
        }

        public ResponseModel<UserEntity> Login(UserLoginRequestModel user)
        {
            if (!emailValidator.IsValidEmail(user.Email))
            {
                logger.LogWarning("Invalid email format for login: {Email}", user.Email);
                return new ResponseModel<UserEntity>
                {
                    Data = null,
                    Success = false,
                    Message = "Invalid email format.",
                    StatusCode = 400 // Bad Request
                };
            }

            // Check Redis cache first
            var cachedUser = redisCache.GetString($"user:{user.Email}");
            UserEntity existingUser = null;
            if (cachedUser != null)
            {
                existingUser = JsonConvert.DeserializeObject<UserEntity>(cachedUser);
            }
            else
            {
                existingUser = addressBookContext.Users.FirstOrDefault(g => g.Email == user.Email);
                if (existingUser != null)
                {
                    // Cache the user data for next use
                    redisCache.SetString($"user:{user.Email}", JsonConvert.SerializeObject(existingUser));
                }
            }

            if (existingUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
            {
                logger.LogWarning("Invalid login attempt for email: {Email}", user.Email);
                return new ResponseModel<UserEntity>
                {
                    Data = null,
                    Success = false,
                    Message = "Invalid credentials.",
                    StatusCode = 401 // Unauthorized
                };
            }

            var userInfo = new UserEntity
            {
                Id = existingUser.Id,
                Name = existingUser.Name,
                Email = existingUser.Email,
                IsAdmin = existingUser.IsAdmin
            };

            return new ResponseModel<UserEntity>
            {
                Data = userInfo,
                Success = true,
                Message = "User Logged in Successfully.",
                StatusCode = 200 // OK
            };
        }

        public void StoreOTP(int otp, string email)
        {
            // Check if OTP exists and reset expiration time
            var cachedOtp = redisCache.GetString($"otp:{email}");
            if (cachedOtp != null)
            {
                redisCache.Remove($"otp:{email}");
            }

            // Store OTP in Redis with an expiration time (e.g., 15 minutes)
            redisCache.SetString($"otp:{email}", otp.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            });

            logger.LogInformation("OTP stored for email: {Email}", email);
        }

        public ResponseModel<string> ResetPassword(ResetPasswordRequestModel request)
        {
            // Check OTP from Redis first
            var cachedOtp = redisCache.GetString($"otp:{request.Email}");

            if (cachedOtp == null || cachedOtp != request.Otp.ToString())
            {
                logger.LogWarning("Invalid or expired OTP for email: {Email}", request.Email);
                return new ResponseModel<string>
                {
                    Data = "Please try again",
                    Success = false,
                    Message = "Wrong or expired OTP",
                    StatusCode = 400 // Bad Request
                };
            }

            // Proceed with password reset
            var user = addressBookContext.Users.FirstOrDefault(g => g.Email == request.Email);
            if (user != null)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                user.Password = hashedPassword;
                user.Otp = 0;
                addressBookContext.SaveChanges();

                // Remove OTP from Redis after reset
                redisCache.Remove($"otp:{request.Email}");

                logger.LogInformation("Password reset successfully for email: {Email}", request.Email);
            }

            return new ResponseModel<string>
            {
                Data = "Done",
                Success = true,
                Message = "Password changed Successfully.",
                StatusCode = 200 // OK
            };
        }
    }
}
