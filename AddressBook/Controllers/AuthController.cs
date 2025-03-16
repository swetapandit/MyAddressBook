using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;

namespace AddressBook.Controllers;

[ApiController]
[Route("/api/[controller]/")]
public class AuthController : ControllerBase
{
    private readonly IUserBL userBL;
    

    public AuthController(IUserBL _userBL)
    {
        userBL = _userBL;
    }

    [HttpPost("register")]
    public IActionResult RegisterUser(UserRegisterRequestModel user)
    {
        var result = userBL.Register(user);
        if (result == null)
        {
            return BadRequest(new { message = "User already exists!" });
        }
        return Ok(result);
    }

    [HttpPost("login")]
    public IActionResult LoginUser(UserLoginRequestModel user)
    {
        var result = userBL.Login(user);
        if (result == null)
        {
            return BadRequest(new { message = "Invalid Credentials!" });
        }

        return Ok(result);
    }
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestModel request)
    {
        var result = await userBL.ForgotPassword(request);
        if (result == null)
        {
            return BadRequest(new { message = "Invalid Credentials!" });
        }

        return Ok(result);
    }

    [HttpPost("reset-password")]
    public  IActionResult ResetPassword(ResetPasswordRequestModel request)
    {
        var result =  userBL.ResetPassword(request);
        if (result == null)
        {
            return BadRequest(new { message = "Invalid Credentials!" });
        }

        return Ok(result);
    }
}