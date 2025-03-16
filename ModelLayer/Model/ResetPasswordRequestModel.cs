using System;
namespace ModelLayer.Model
{
	public class ResetPasswordRequestModel
	{
        public string Email { get; set; }
        public int Otp { get; set; }
        public string Password { get; set; }
    }
}

