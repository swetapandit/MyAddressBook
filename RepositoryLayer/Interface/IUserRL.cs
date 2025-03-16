using System;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        ResponseModel<UserEntity> Register(UserRegisterRequestModel user);
        ResponseModel<UserEntity> Login(UserLoginRequestModel user);
        void StoreOTP(int otp, string email);
        ResponseModel<string> ResetPassword(ResetPasswordRequestModel request);
    }
}