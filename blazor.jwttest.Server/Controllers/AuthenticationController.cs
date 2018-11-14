using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using blazor.jwttest.Server.Services;
using blazor.jwttest.Server.Services.Exceptions;
using blazor.jwttest.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace gisportal.Server.Controllers
{
  [Route("api/[controller]")]
  public class AuthenticationController : Controller
  {
    private readonly IConfiguration _configuration;
    private readonly Users _users;

    public AuthenticationController(
      IConfiguration configuration,
      Users users
      )
    {
      _configuration = configuration;
      _users = users;
    }

    [HttpPost("[action]")]
    public IActionResult Login([FromBody] LoginDetails login)
    {
      User thisUser = null;
      try
      {
        thisUser = _users.ValidateLogin(login.Username, login.Password);
      }
      catch (UserNotValidatedException)
      {
        // NOTE: If you want to check the exception at this point, there's a validation reason on it that tells you
        // what actually failed, however it's generally good practice not to tell that info to the UI as it gives
        // anyone trying to gain access maliciously and idea of what's right and what's not :-)
        return BadRequest("Username and password are invalid.");
      }

      var claims = new List<Claim>()
      {
        new Claim(ClaimTypes.Name, thisUser.LoginName),
        new Claim(ClaimTypes.GivenName, thisUser.FullName),
        new Claim(ClaimTypes.Email, thisUser.Email)
      };

      foreach(var role in thisUser.AllowedRoles)
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var expiry = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtExpiryInMinutes"]));
      var token = new JwtSecurityToken(
          _configuration["JwtIssuer"],
          _configuration["JwtIssuer"],
          claims,
          expires: expiry,
          signingCredentials: creds
      );

      return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });

    }

  }
}