using System;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Model
{
    public class UserRegisterRequestModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}