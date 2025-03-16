using System;
using ModelLayer.Model;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        private readonly AddressBookContext addressBookContext;
        private readonly EmailValidator emailValidator;
        public UserRL(AddressBookContext _addressBookContext,EmailValidator _emailValidator)
        {
            addressBookContext = _addressBookContext;
            emailValidator = _emailValidator;
        }

        public ResponseModel<UserEntity> Register(UserRegisterRequestModel user)
        {
            if (!emailValidator.IsValidEmail(user.Email))
            {
                return new ResponseModel<UserEntity>
                {
                    Data = null,
                    Success = false,
                    Message = "Invalid email format.",
                    StatusCode = 400 // Bad Request
                };
            }
            var existingUser = addressBookContext.Users.FirstOrDefault(g => g.Email == user.Email);

            if (existingUser != null) return null;

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
                return new ResponseModel<UserEntity>
                {
                    Data = null,
                    Success = false,
                    Message = "Invalid email format.",
                    StatusCode = 400 // Bad Request
                };
            }

            var existingUser = addressBookContext.Users.FirstOrDefault(g => g.Email == user.Email);

            if (existingUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
            {
                return null; // Invalid credentials
            }

            var existingUser1 = new UserEntity
            {
                Id = existingUser.Id,   // ID ko include karo
                Name = existingUser.Name,
                Email = existingUser.Email,
                IsAdmin = existingUser.IsAdmin
            };

            return new ResponseModel<UserEntity>
            {
                Data = existingUser1,
                Success = true,
                Message = "User Logged in Successfully.",
                StatusCode = 200 // OK
            };
        }
        public void StoreOTP(int otp,string email)
        {
            var user = addressBookContext.Users.FirstOrDefault(c => c.Email == email);
            user.Otp = otp;
            addressBookContext.SaveChanges();
        }

        public ResponseModel<string> ResetPassword(ResetPasswordRequestModel request)
        {
            var user = addressBookContext.Users.FirstOrDefault(g => g.Email == request.Email);
            if(user.Otp != request.Otp)
            {
                return new ResponseModel<string>
                {
                    Data = "Please try again",
                    Success = true,
                    Message = "Wrong or expired otp",
                    StatusCode = 400 // OK
                };
            }
            //it means otp is matched
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.Password = hashedPassword;
            user.Otp = 0;
            addressBookContext.SaveChanges();

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